namespace WebApp.Models;

public class User
{
  public string? Email { get; set; }
  public List<ToDoItem>? ToDoItems { get; set; }
  public string? CPR { get; set; }
}
