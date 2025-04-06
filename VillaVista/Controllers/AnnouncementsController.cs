using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using VillaVista.Data;

public class AnnouncementsController : Controller
{
    private readonly HomeownerDbContext _context;

    public AnnouncementsController(HomeownerDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var announcements = await _context.Announcements.OrderByDescending(a => a.CreatedAt).ToListAsync();
        return View(announcements);
    }

    public IActionResult Create() => View();

    [HttpPost]
    public async Task<IActionResult> Create(Announcement announcement)
    {
        if (ModelState.IsValid)
        {
            _context.Announcements.Add(announcement);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(announcement);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var announcement = await _context.Announcements.FindAsync(id);
        if (announcement == null) return NotFound();
        return View(announcement);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, Announcement announcement)
    {
        if (id != announcement.Id) return NotFound();

        if (ModelState.IsValid)
        {
            _context.Update(announcement);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(announcement);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var announcement = await _context.Announcements.FindAsync(id);
        if (announcement == null) return NotFound();

        _context.Announcements.Remove(announcement);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
