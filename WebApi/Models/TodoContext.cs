using Microsoft.EntityFrameworkCore;

namespace WebApi.Models;

public class TodoContext(DbContextOptions<TodoContext> options) : DbContext(options)
{
  public DbSet<ToDoItem> TodoItems { get; set; }

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    optionsBuilder.UseSqlite("Data Source=TodoDb.db");
  }
}
