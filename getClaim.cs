=== Test as params ===
public status int getUserId(ClaimsPrincipal User){
        var idClaim = User.Claims.FirstOrDefault(c => c.Type.Equals("Id", StringComparison.InvariantCultureIgnoreCase));
            if (idClaim != null)
            {
                var id = int.Parse(idClaim.Value);
                return id;
            }
            return -1;
}

public status int getUserRole(ClaimsPrincipal User){
        var idClaim = User.Claims.FirstOrDefault(c => c.Type.Equals("Role", StringComparison.InvariantCultureIgnoreCase));
            if (idClaim != null)
            {
                var id = int.Parse(idClaim.Value);
                return id;
            }
            return -1;
}


======================================
        
        
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

=====Ref 1=====
Inject IHttpContextAccessor interface into the target class. This will give access to the current User via the HttpContext

This provides an opportunity to abstract this feature by creating a service to provide just the information you want (Which is the current logged in user)

public interface IUserService {
    ClaimsPrincipal GetUser();
}

public class UserService : IUserService {
    private readonly IHttpContextAccessor accessor;

    public UserService(IHttpContextAccessor accessor) {
        this.accessor = accessor;
    }

    public ClaimsPrincipal GetUser() {
        return accessor?.HttpContext?.User as ClaimsPrincipal;
    }
}
You need to setup IHttpContextAccessor now in Startup.ConfigureServices in order to be able to inject it:

services.AddHttpContextAccessor();
services.AddTransient<IUserService, UserService>();
and inject your service where needed.

===== Ref 2 ========
create a service

public class UserResolverService
{
    private readonly IHttpContextAccessor _context;

    public UserResolverService(IHttpContextAccessor context)
    {
        _context = context;
    }

    public string GetUser()
    {
       return await _context.HttpContext.User?.Identity?.Name;
    }
}
in your startup.cs ConfigureServices method add

services.AddTransient<UserResolverService>();
Then just pass it into your implementation of your specified DbContext

public partial class ExampleContext : IExampleContext
{
    private YourContext _context;
    private string _user;

    public ExampleContext(YourContext context, UserResolverService userService)
    {
        _context = context;
        _user = userService.GetUser();
    }
}
