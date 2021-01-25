namespace API.Controllers
{

    [Route("api/[controller]")]

    public class AuthController : Controller
    {
        private readonly IConfiguration config;
        private readonly IUserRepository userRepository;
        private readonly IMapper mapper;
        private IUserModuleRightsRepository userModuleRepository;
        private IModuleRightsRepository moduleRightsRepository;

        public IUserModuleRightsRepository userModuleRightsRepository;

        private readonly IUnitOfWork unitOfWork;

        private bool shouldLoginAD = false;
        private string domainName = "";
        private bool createNewuser = false;
        private string licensekey = "";

        public AuthController(
          IUserRepository userRepository,
          IUnitOfWork unitOfWork,
          IMapper mapper,
          IUserModuleRightsRepository userModuleRepository,
          IModuleRightsRepository moduleRightsRepository,
          IConfiguration config,
          IUserModuleRightsRepository userModuleRightsRepository)
        {
            this.userRepository = userRepository;
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.userModuleRepository = userModuleRepository;
            this.moduleRightsRepository = moduleRightsRepository;
            this.userModuleRightsRepository = userModuleRightsRepository;
            this.config = config;
            this.shouldLoginAD = config.GetValue<bool>("LoginAD");
            this.domainName = config.GetSection("Domain").Value;
            this.licensekey = config.GetSection("LicenseKey").Value;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginResource loginResource)
        {

            var username = loginResource.Username;
            var password = loginResource.Password;

            var anyUser = await userRepository.GetAll();
            var userLogin = await userRepository.GetOneByUserName(username);

            if (shouldLoginAD)
            {
                try
                {
                    var de = new DirectoryEntry("LDAP://" + domainName, username, password);
                    var ds = new DirectorySearcher(de);
                    SearchResult search = ds.FindOne();

                    if (search != null)
                    {
                        if (userLogin == null)
                        {
                            createNewuser = true;
                        }
                    }
                    else
                    {
                        return Unauthorized();
                    }
                }
                catch
                {
                    return BadRequest("AD Login Problem");
                }

                if (createNewuser)
                {
                    // new user here
                    userLogin = new User()
                    {
                        Username = username,
                        GddbId = username,
                        AdminStatus = anyUser.Count() <= 0 ? true : false,
                        isActive = true
                    };
                    password = "";
                    userRepository.Add(userLogin, password);

                    // Copy all right to module right
                    var rightLists = await moduleRightsRepository.GetAll();
                    foreach (ModuleRight list in rightLists)
                    {
                        var userModuleRights = new UserModuleRight
                        {
                            ModuleRightsId = list.Id,
                            UserId = userLogin.Id,
                            Read = false,
                            Write = false
                        };

                        var saveUserModule = mapper.Map<UserModuleRight>(userModuleRights);

                        userModuleRightsRepository.Add(saveUserModule);
                    }

                    if (await unitOfWork.CompleteAsync() == false)
                    {
                        throw new Exception(message: "Save new user Failed");
                    }
                }
            }
            else
            {
                userLogin = await userRepository.Login(username, password);
            }

            if (userLogin == null)
                return Unauthorized();

            var allUserModules = await userModuleRepository.GetAll();
            var userModules = allUserModules.Where(u => u.UserId == userLogin.Id).ToList();

            // Add user claim
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, userLogin.Username));
            claims.Add(new Claim("Id", userLogin.Id.ToString()));

            if (userLogin.AdminStatus == true)
            {
                claims.Add(new Claim(ClaimTypes.Role, "Administrator"));
            }

            foreach (UserModuleRight userModule in userModules)
            {
                var right = await moduleRightsRepository.GetOne(userModule.ModuleRightsId);
                var claim = right.Description.ToString();

                if (userModule.Read == true)
                {
                    claims.Add(new Claim(ClaimTypes.Role, $"{claim}.R"));
                }

                if (userModule.Write == true)
                {
                    claims.Add(new Claim(ClaimTypes.Role, $"{claim}.W"));
                }
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(this.config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var user = mapper.Map<ViewUserResource>(userLogin);

            return Ok(new
            {
                token = tokenHandler.WriteToken(token),
                user
            });
        }
    }
}
