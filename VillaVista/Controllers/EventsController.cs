using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using VillaVista.Data;

public class EventsController : Controller
{
    private readonly HomeownerDbContext _context;

    public EventsController(HomeownerDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var events = await _context.Events.OrderBy(e => e.EventDate).ToListAsync();
        return View(events);
    }

    public IActionResult Create() => View();

    [HttpPost]
    public async Task<IActionResult> Create(Event eventItem)
    {
        if (ModelState.IsValid)
        {
            _context.Events.Add(eventItem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(eventItem);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var eventItem = await _context.Events.FindAsync(id);
        if (eventItem == null) return NotFound();
        return View(eventItem);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, Event eventItem)
    {
        if (id != eventItem.Id) return NotFound();

        if (ModelState.IsValid)
        {
            _context.Update(eventItem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(eventItem);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var eventItem = await _context.Events.FindAsync(id);
        if (eventItem == null) return NotFound();

        _context.Events.Remove(eventItem);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
