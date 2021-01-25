
namespace API.Persistence.Repository
{
  public class UserModuleRightsRepository : IUserModuleRightsRepository
  {
    private readonly DataContext context;

    public UserModuleRightsRepository(DataContext context)
    {
      this.context = context;
    }

    public async Task<UserModuleRight> GetOne(int id)
    {
      return await context.UserModuleRight.FindAsync(id);
    }

    public void Add(UserModuleRight userModuleRights)
    {
      context.UserModuleRight.Add(userModuleRights);
    }

    public void Remove(UserModuleRight userModuleRights)
    {
      context.Remove(userModuleRights);
    }

    public async Task<IEnumerable<UserModuleRight>> GetAll()
    {
      var usermodulergihts = await context.UserModuleRight.ToListAsync();
      return usermodulergihts;
    }

    public async Task<PagedList<UserModuleRight>> GetPagedUserModuleRights(UserModuleRightsParams userModuleRightsParams)
    {
      var usermodulerights = context.UserModuleRight.AsQueryable();

      // filter
      if (userModuleRightsParams.UserId > 0)
      {
        usermodulerights = usermodulerights.Where(u => u.UserId == userModuleRightsParams.UserId);
      }

      if (userModuleRightsParams.ModuleRightsId > 0)
      {
        usermodulerights = usermodulerights.Where(u => u.ModuleRightsId == userModuleRightsParams.ModuleRightsId);
      }

      // sort
      if (userModuleRightsParams.isDescending)
      {
        if (!string.IsNullOrEmpty(userModuleRightsParams.OrderBy))
        {
          switch (userModuleRightsParams.OrderBy.ToLower())
          {
            case "userid":
              usermodulerights = usermodulerights.OrderByDescending(u => u.UserId);
              break;
            case "modulerightsid":
              usermodulerights = usermodulerights.OrderByDescending(u => u.ModuleRightsId);
              break;
            default:
              usermodulerights = usermodulerights.OrderByDescending(u => u.UserId);
              break;
          }
        }
        else
        {
          usermodulerights = usermodulerights.OrderByDescending(u => u.UserId);
        }
      }
      else
      {
        if (!string.IsNullOrEmpty(userModuleRightsParams.OrderBy))
        {
          switch (userModuleRightsParams.OrderBy.ToLower())
          {
            case "userid":
              usermodulerights = usermodulerights.OrderBy(u => u.UserId);
              break;
            case "modulerightsid":
              usermodulerights = usermodulerights.OrderBy(u => u.ModuleRightsId);
              break;
            default:
              usermodulerights = usermodulerights.OrderBy(u => u.UserId);
              break;
          }
        }
        else
        {
          usermodulerights = usermodulerights.OrderBy(u => u.UserId);
        }
      }

      return await PagedList<UserModuleRight>.CreateAsync(usermodulerights, userModuleRightsParams.PageNumber, userModuleRightsParams.PageSize);
    }
  }
}
