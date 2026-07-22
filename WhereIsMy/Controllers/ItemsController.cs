using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WhereIsMy;

namespace whereIsMy;
public class ItemsController : Controller
{
    private readonly AppDbContext _context;

    public ItemsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet ("/items")]
    public async Task<IActionResult> Index()
    {
        var applicationDbContext = _context.Items.Include(i => i.Location).Include(i => i.User);
        return View(await applicationDbContext.ToListAsync());
    }
    }