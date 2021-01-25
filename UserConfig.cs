namespace API.Persistence.Configuration
{
  public class UserConfiguration : IEntityTypeConfiguration<User>
  {
    public void Configure(EntityTypeBuilder<User> builder)
    {
      builder
        .HasIndex(u => u.Username);

      builder
        .HasIndex(u => u.FirstName);

      builder
        .HasIndex(u => u.LastName);

      builder
        .HasMany<UserDepartment>(u => u.UserDepartments)
        .WithOne(ud => ud.User)
        .HasForeignKey(ud => ud.UserId)
        .OnDelete(DeleteBehavior.Cascade);

      builder
        .HasMany<UserModuleRight>(u => u.UserModuleRights)
        .WithOne(um => um.User)
        .HasForeignKey(um => um.UserId)
        .OnDelete(DeleteBehavior.Cascade);
    }
  }
}
