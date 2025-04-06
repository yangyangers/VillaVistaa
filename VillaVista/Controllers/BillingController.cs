using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using VillaVista.Data;

public class BillingController : Controller
{
    private readonly HomeownerDbContext _context;

    public BillingController(HomeownerDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var bills = await _context.Billings.OrderBy(b => b.DueDate).ToListAsync();
        return View(bills);
    }

    public async Task<IActionResult> Pay(int id)
    {
        var bill = await _context.Billings.FindAsync(id);
        if (bill == null) return NotFound();

        bill.IsPaid = true;
        bill.PaidAt = System.DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
