
namespace API.Controllers
{
  [Route("api/[controller]")]
  public class UserModuleRightsController : Controller
  {
    private readonly IMapper mapper;
    private IUserModuleRightsRepository userModuleRightRepository;
    private IUnitOfWork unitOfWork;

    public UserModuleRightsController(IMapper mapper, IUserModuleRightsRepository userModuleRightRepository, IUnitOfWork unitOfWork)
    {
      this.mapper = mapper;
      this.userModuleRightRepository = userModuleRightRepository;
      this.unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
      var usermodulerights = await userModuleRightRepository.GetAll();

      var result = mapper.Map<IEnumerable<ViewUserModuleRightsResource>>(usermodulerights);

      return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOne(int id)
    {
      var usermoduleright = await userModuleRightRepository.GetOne(id);

      if (usermoduleright == null)
        return NotFound();

      var result = mapper.Map<UserModuleRight, ViewUserModuleRightsResource>(usermoduleright);

      return Ok(result);
    }

    [HttpGet("paged")]
    public async Task<IActionResult> GetPagedUserModuleRights([FromQuery]UserModuleRightsParams userModuleRightsParams)
    {
      var usermodulerights = await userModuleRightRepository.GetPagedUserModuleRights(userModuleRightsParams);

      var result = mapper.Map<IEnumerable<ViewUserModuleRightsResource>>(usermodulerights);

      Response.AddPagination(usermodulerights.CurrentPage, usermodulerights.PageSize, usermodulerights.TotalCount, usermodulerights.TotalPages);

      return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody]SaveUserModuleRightsResource userModuleRightsResource)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      var usermoduleright = mapper.Map<SaveUserModuleRightsResource, UserModuleRight>(userModuleRightsResource);

      userModuleRightRepository.Add(usermoduleright);

      if (await unitOfWork.CompleteAsync() == false)
      {
        throw new Exception(message: "Create new user module right failed on save");
      }

      usermoduleright = await userModuleRightRepository.GetOne(usermoduleright.Id);

      var result = mapper.Map<UserModuleRight, ViewUserModuleRightsResource>(usermoduleright);

      return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody]SaveUserModuleRightsResource userModuleRightsResource)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      var usermoduleright = await userModuleRightRepository.GetOne(id);

      if (usermoduleright == null)
        return NotFound();

      usermoduleright = mapper.Map(userModuleRightsResource, usermoduleright);

      if (await unitOfWork.CompleteAsync() == false)
      {
        throw new Exception(message: $"Updating user module right with id: {id} failed on save");
      }

      usermoduleright = await userModuleRightRepository.GetOne(usermoduleright.Id);

      var result = mapper.Map<UserModuleRight, ViewUserModuleRightsResource>(usermoduleright);

      return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveUserModuleRight(int id)
    {
      var usermoduleright = await userModuleRightRepository.GetOne(id);

      if (usermoduleright == null)
        return NotFound();

      userModuleRightRepository.Remove(usermoduleright);

      if (await unitOfWork.CompleteAsync() == false)
      {
        throw new Exception(message: $"Deleting user module right failed");
      }

      return Ok($"{id}");
    }

    // FIXME : make me to be reuseable
    private int getUserId()
    {
      var idClaim = User.Claims.FirstOrDefault(c => c.Type.Equals("Id", StringComparison.InvariantCultureIgnoreCase));
      if (idClaim != null)
      {
        var id = int.Parse(idClaim.Value);
        return id;
      }
      return -1;
    }
  }
}
