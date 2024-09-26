using Microsoft.EntityFrameworkCore;

namespace WebApi.Models;

public class UserContext(DbContextOptions<UserContext> options) : DbContext(options)
{
  public DbSet<User> Users { get; set; }
}
