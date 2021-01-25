namespace API.Core.IRepository
{
  public interface IUserDepartmentRepository
  {
    Task<UserDepartment> GetOne(int id);
    void Add(UserDepartment userDepartment);
    void Remove(UserDepartment userDepartment);
    Task<IEnumerable<UserDepartment>> GetAll();
    Task<PagedList<UserDepartment>> GetPagedUserDepartment(UserDepartmentParams userDepartmentParams);
  }
}
