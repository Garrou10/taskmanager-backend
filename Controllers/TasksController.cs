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

    [HttpPost("{id}/upload")]
    public async Task<IActionResult> UploadImage(int id, IFormFile file)
    {
        var taskItem = await _context.Tasks.FindAsync(id);
        if (taskItem == null) return NotFound("Uppgiften hittades inte.");
        if (file == null || file.Length == 0) return BadRequest("Ingen fil valdes.");

        // Skapar en "wwwroot/uploads" mapp om den inte redan finns
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

        // Ger filen ett unikt namn och sparar den
        var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Uppdaterar uppgiftens ImageUrl i databasen
        taskItem.ImageUrl = $"/uploads/{uniqueFileName}";
        await _context.SaveChangesAsync();

        return Ok(taskItem);
    }
}

