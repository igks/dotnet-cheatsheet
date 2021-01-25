
namespace API.Persistence.Repository
{
  public class UserDepartmentRepository : IUserDepartmentRepository
  {
    private readonly DataContext context;

    public UserDepartmentRepository(DataContext context)
    {
      this.context = context;
    }

    public async Task<UserDepartment> GetOne(int id)
    {
      return await context.UserDepartment.FindAsync(id);
    }

    public void Add(UserDepartment userDepartment)
    {
      context.UserDepartment.Add(userDepartment);
    }

    public void Remove(UserDepartment userDepartment)
    {
      context.Remove(userDepartment);
    }

    public async Task<IEnumerable<UserDepartment>> GetAll()
    {
      var userDepartments = await context.UserDepartment.ToListAsync();

      return userDepartments;
    }

    public async Task<PagedList<UserDepartment>> GetPagedUserDepartment(UserDepartmentParams userDepartmentParams)
    {
      var userDepartments = context.UserDepartment.AsQueryable();

      if (userDepartmentParams.UserId > 0)
      {
        userDepartments = userDepartments.Where(u => u.UserId == userDepartmentParams.UserId);
      }

      if (userDepartmentParams.DepartmentId > 0)
      {
        userDepartments = userDepartments.Where(u => u.DepartmentId == userDepartmentParams.DepartmentId);
      }

      // sort
      if (userDepartmentParams.isDescending)
      {
        if (!string.IsNullOrEmpty(userDepartmentParams.OrderBy))
        {
          switch (userDepartmentParams.OrderBy.ToLower())
          {
            case "userid":
              userDepartments = userDepartments.OrderByDescending(u => u.UserId);
              break;
            case "departmentid":
              userDepartments = userDepartments.OrderByDescending(u => u.DepartmentId);
              break;
            default:
              userDepartments = userDepartments.OrderByDescending(u => u.UserId);
              break;
          }
        }
        else
        {
          userDepartments = userDepartments.OrderByDescending(u => u.UserId);
        }
      }
      else
      {
        if (!string.IsNullOrEmpty(userDepartmentParams.OrderBy))
        {
          switch (userDepartmentParams.OrderBy.ToLower())
          {
            case "userid":
              userDepartments = userDepartments.OrderBy(u => u.UserId);
              break;
            case "departmentid":
              userDepartments = userDepartments.OrderBy(u => u.DepartmentId);
              break;
            default:
              userDepartments = userDepartments.OrderBy(u => u.UserId);
              break;
          }
        }
        else
        {
          userDepartments = userDepartments.OrderBy(u => u.UserId);
        }
      }

      return await PagedList<UserDepartment>.CreateAsync(userDepartments, userDepartmentParams.PageNumber, userDepartmentParams.PageSize);
    }
  }
}
