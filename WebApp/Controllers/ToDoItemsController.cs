using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;

namespace WebApp.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class ToDoItemsController : ControllerBase
  {
    private readonly TodoContext _context;

    public ToDoItemsController(TodoContext context)
    {
      _context = context;
    }

    // GET: api/ToDoItems
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ToDoItem>>> GetTodoItems()
    {
      return await _context.TodoItems.ToListAsync();
    }

    // GET: api/ToDoItems/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ToDoItem>> GetToDoItem(int id)
    {
      var toDoItem = await _context.TodoItems.FindAsync(id);

      if (toDoItem == null)
      {
        return NotFound();
      }

      return toDoItem;
    }

    // PUT: api/ToDoItems/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutToDoItem(int id, ToDoItem toDoItem)
    {
      if (id != toDoItem.Id)
      {
        return BadRequest();
      }

      _context.Entry(toDoItem).State = EntityState.Modified;

      try
      {
        await _context.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException)
      {
        if (!ToDoItemExists(id))
        {
          return NotFound();
        }
        else
        {
          throw;
        }
      }

      return NoContent();
    }

    // POST: api/ToDoItems
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<ToDoItem>> PostToDoItem(ToDoItem toDoItem)
    {
      toDoItem = toDoItem with { Id = 0 };
      _context.TodoItems.Add(toDoItem);
      await _context.SaveChangesAsync();

      return CreatedAtAction("GetToDoItem", new { id = toDoItem.Id }, toDoItem);
    }

    // DELETE: api/ToDoItems/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteToDoItem(int id)
    {
      var toDoItem = await _context.TodoItems.FindAsync(id);
      if (toDoItem == null)
      {
        return NotFound();
      }

      _context.TodoItems.Remove(toDoItem);
      await _context.SaveChangesAsync();

      return NoContent();
    }

    private bool ToDoItemExists(int id)
    {
      return _context.TodoItems.Any(e => e.Id == id);
    }
  }
}
