using System.ComponentModel.DataAnnotations;

namespace WebApi.Models;

public class User
{
  [Key]
  public string Email { get; set; }
  public List<ToDoItem>? ToDoItems { get; set; }
  public string? CPR { get; set; }
}
