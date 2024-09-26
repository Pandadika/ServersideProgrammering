namespace WebApp.Models;

public record class ToDoItem
{
  public int Id { get; init; }
  public required string Title { get; set; }
  public bool IsDone { get; set; }
}
