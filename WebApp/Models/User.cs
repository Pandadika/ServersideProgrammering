using WebApp.Data;

namespace WebApp.Models;

public class User : ApplicationUser
{
  public required string Email { get; set; }
  public List<ToDoItem>? ToDoItems { get; set; }
  public required string CPR { get; set; }
}
