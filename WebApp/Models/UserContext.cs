using Microsoft.EntityFrameworkCore;

namespace WebApp.Models;

public class UserContext(DbContextOptions<UserContext> options) : DbContext(options)
{
  public DbSet<User> Users { get; set; }
}
