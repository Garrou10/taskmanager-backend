using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagerApi.Data;
using TaskManagerApi.Models;

namespace TaskManagerApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TasksController : ControllerBase
{
    private readonly AppDbContext _context;

    public TasksController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/tasks (Hämtar alla uppgifter)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskItem>>> GetTasks()
    {
        return await _context.Tasks.ToListAsync();
    }

    // POST: api/tasks (Skapar en ny uppgift)
    [HttpPost]
    public async Task<ActionResult<TaskItem>> PostTask(TaskItem taskItem)
    {
        _context.Tasks.Add(taskItem);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTasks), new { id = taskItem.Id }, taskItem);
    }

    // PUT: api/tasks/5 (Uppdaterar en befintlig uppgift)
    [HttpPut("{id}")]
    public async Task<IActionResult> PutTask(int id, TaskItem taskItem)
    {
        if (id != taskItem.Id)
        {
            return BadRequest("ID i URL matchar inte ID i datan.");
        }

        _context.Entry(taskItem).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }
}