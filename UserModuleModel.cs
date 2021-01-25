namespace API.Core.Models
{
  public class UserModuleRight
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int ModuleRightsId { get; set; }
    public ModuleRight ModuleRights { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public bool Read { get; set; }
    public bool Write { get; set; }
  }
}
