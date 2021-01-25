namespace API.Core.Models
{
  public class UserDepartment
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public Department Department { get; set; }
    public int DepartmentId { get; set; }

    public User User { get; set; }
    public int UserId { get; set; }
  }
}
