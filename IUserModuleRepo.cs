namespace BusMeal.API.Core.IRepository
{
  public interface IUserModuleRightsRepository
  {
    Task<UserModuleRight> GetOne(int id);
    void Add(UserModuleRight userModuleRights);
    void Remove(UserModuleRight userModuleRights);
    Task<IEnumerable<UserModuleRight>> GetAll();
    Task<PagedList<UserModuleRight>> GetPagedUserModuleRights(UserModuleRightsParams userModuleRightsParams);

  }
}
