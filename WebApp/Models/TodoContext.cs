using Microsoft.EntityFrameworkCore;

namespace WebApp.Models;

public class TodoContext(DbContextOptions<TodoContext> options) : DbContext(options)
{
  public DbSet<ToDoItem> TodoItems { get; set; }
}
