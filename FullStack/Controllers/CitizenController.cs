using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FullStack.Models;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

[Authorize]
public class CitizenController : Controller
{
    private readonly FullStackDbContext _context;

    public CitizenController(FullStackDbContext context)
    {
        _context = context;
    }

   
    public async Task<IActionResult> Index(string searchString)
    {
        ViewData["CurrentFilter"] = searchString;
        IQueryable<Citizen> query = _context.Citizens;

        if (!String.IsNullOrEmpty(searchString))
        {
            query = query.Where(c => c.CitizenName.Contains(searchString) || c.CitizenID.Contains(searchString) || c.Nationality.Contains(searchString));
        }

        if (!User.IsInRole("Admin"))
        {
            var userCitizenId = User.FindFirst("CitizenID")?.Value;
            query = query.Where(c => c.CitizenID == userCitizenId);
        }
        return View(await query.ToListAsync());
    }


    public async Task<IActionResult> Details(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var model = new CitizenDetailsViewModel();
        model.Citizen = await _context.Citizens.FirstOrDefaultAsync(m => m.CitizenID == id);

        if (model.Citizen == null)
        {
            return NotFound();
        }

        if (!User.IsInRole("Admin") && model.Citizen.CitizenID != User.FindFirst("CitizenID")?.Value)
        {
            return RedirectToAction("Index", "Home");
        }

        model.Addresses = await _context.Addresses.Where(a => a.CitizenID == id).ToListAsync();
        model.Documents = await _context.Documents.Where(d => d.CitizenID == id).ToListAsync();
        model.Records = await _context.CitizenRecords.Where(r => r.CitizenID == id).OrderByDescending(r => r.RecordDate).ToListAsync();

        return View(model);
    }

   
    public IActionResult Create()
    {
        return View();
    }

   
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Citizen citizen)
    {
        if (!ModelState.IsValid)
        {
            return View(citizen);
        }

        citizen.CitizenID =
            (citizen.CitizenName?.Length >= 2 ? citizen.CitizenName.Substring(0, 2).ToUpper() : "NA")
            + "-" +
            (citizen.Nationality?.Length >= 2 ? citizen.Nationality.Substring(0, 2).ToUpper() : "NA")
            + "-" +
            (citizen.Gender?.Length >= 2 ? citizen.Gender.Substring(0, 2).ToUpper() : "NA")
            + "-" +
            (citizen.Language?.Length >= 2 ? citizen.Language.Substring(0, 2).ToUpper() : "NA");

        try
        {
            _context.Citizens.Add(citizen);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(citizen);
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: Citizen/Edit/5
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(string id)
    {
        if (id == null) return NotFound();

        var citizen = await _context.Citizens.FindAsync(id);
        if (citizen == null) return NotFound();

        return View(citizen);
    }

    // POST: Citizen/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(string id, Citizen citizen)
    {
        if (id != citizen.CitizenID) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(citizen);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CitizenExists(citizen.CitizenID)) return NotFound();
                else throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(citizen);
    }


    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(string id)
    {
        if (id == null) return NotFound();

        var citizen = await _context.Citizens.FirstOrDefaultAsync(m => m.CitizenID == id);
        if (citizen == null) return NotFound();

        return View(citizen);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        var citizen = await _context.Citizens.FindAsync(id);
        if (citizen != null)
        {
            var records = _context.CitizenRecords.Where(r => r.CitizenID == id);
            _context.CitizenRecords.RemoveRange(records);

          var documents = _context.Documents.Where(d => d.CitizenID == id);
            _context.Documents.RemoveRange(documents);

           
            var addresses = _context.Addresses.Where(a => a.CitizenID == id);
            _context.Addresses.RemoveRange(addresses);

            var login = await _context.Logins.FirstOrDefaultAsync(l => l.LoginID == id);
            if (login != null)
            {
                _context.Logins.Remove(login);
            }

          
            _context.Citizens.Remove(citizen);

            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    private bool CitizenExists(string id)
    {
        return _context.Citizens.Any(e => e.CitizenID == id);
    }
}