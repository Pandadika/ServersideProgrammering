using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Models;

namespace WebApi.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class UsersController : ControllerBase
  {
    private readonly UserContext _context;

    public UsersController(UserContext context)
    {
      _context = context;
    }

    // GET: api/Users
    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
      return await _context.Users.ToListAsync();
    }

    // GET: api/Users/5
    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser(string id)
    {
      var user = await _context.Users.FindAsync(id);

      if (user == null)
      {
        return NotFound();
      }

      return user;
    }

    // POST: api/Users
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<User>> PostUser(User user)
    {
      _context.Users.Add(user);
      try
      {
        await _context.SaveChangesAsync();
      }
      catch (DbUpdateException)
      {
        if (UserExists(user.Email))
        {
          return Conflict();
        }
        else
        {
          throw;
        }
      }

      return CreatedAtAction("GetUser", new { id = user.Email }, user);
    }

    [HttpPut("cpr/{id}")]
    public async Task<ActionResult<bool>> CheckCpr(string id, User user)
    {
      if (id != user.Email)
      {
        return BadRequest();
      }

      if (user.CPR == null)
      {
        return BadRequest();
      }

      var dbuser = await _context.Users.FindAsync(id);
      if (dbuser == null)
      {
        return NotFound();
      }
      if (dbuser.CPR == null)
      {
        dbuser.CPR = user.CPR;
        await _context.SaveChangesAsync();
        return Ok(true);
      }
      else
      {
        if (dbuser.CPR == user.CPR)
        {
          return Ok(true);
        }
        else
        {
          return Ok(false);
        }
      }
    }

    [HttpGet("todoitems/{id}")]
    public async Task<ActionResult<List<ToDoItem>>> GetToDoItems(string id)
    {
      var user = await _context.Users
                               .Include(u => u.ToDoItems)
                               .FirstOrDefaultAsync(u => u.Email == id);
      if (user == null)
      {
        return NotFound();
      }
      user.ToDoItems ??= [];

      return Ok(user.ToDoItems);
    }

    [HttpPost("todoitems/{id}")]
    public async Task<ActionResult<bool>> PostToDoItem(string id, ToDoItem toDoItem)
    {
      var user = await _context.Users
                               .Include(u => u.ToDoItems)
                               .FirstOrDefaultAsync(u => u.Email == id);
      if (user == null)
      {
        return NotFound();
      }
      user.ToDoItems ??= new();
      user.ToDoItems.Add(toDoItem);
      await _context.SaveChangesAsync();
      return Ok(true);
    }

    [HttpDelete("todoitems/{id}")]
    public async Task<ActionResult<bool>> DeleteToDoItem(string id, ToDoItem toDoItem)
    {
      var user = await _context.Users
                               .Include(u => u.ToDoItems)
                               .FirstOrDefaultAsync(u => u.Email == id);
      if (user == null)
      {
        return NotFound();
      }
      user.ToDoItems ??= new();
      var item = user.ToDoItems.FirstOrDefault(i => i.Id == toDoItem.Id);
      if (item == null)
      {
        return NotFound();
      }
      user.ToDoItems.Remove(item);
      await _context.SaveChangesAsync();
      return Ok(true);
    }

    [HttpPut("todoitems/{id}")]
    public async Task<ActionResult<bool>> SetDone(string id, ToDoItem toDoItem)
    {
      var user = await _context.Users
                               .Include(u => u.ToDoItems)
                               .FirstOrDefaultAsync(u => u.Email == id);
      if (user == null)
      {
        return NotFound();
      }
      user.ToDoItems ??= new();
      var item = user.ToDoItems.FirstOrDefault(i => i.Id == toDoItem.Id);
      if (item == null)
      {
        return NotFound();
      }
      item.IsDone = true;
      await _context.SaveChangesAsync();
      return Ok(true);
    }


    // DELETE: api/Users/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
      var user = await _context.Users.FindAsync(id);
      if (user == null)
      {
        return NotFound();
      }

      _context.Users.Remove(user);
      await _context.SaveChangesAsync();

      return NoContent();
    }

    private bool UserExists(string id)
    {
      return _context.Users.Any(e => e.Email == id);
    }
  }
}
