using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using FullStack.Models;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace FullStack.Controllers
{
    [Authorize]
    public class AddressController : Controller
    {
        private readonly FullStackDbContext _context;

        public AddressController(FullStackDbContext context)
        {
            _context = context;
        }

        // GET: Address
        public async Task<IActionResult> Index(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;
            IQueryable<AddressRecord> query = _context.Addresses;
            if (!User.IsInRole("Admin"))
            {
                var userCitizenId = User.FindFirst("CitizenID")?.Value;
                query = query.Where(a => a.CitizenID == userCitizenId);
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                query = query.Where(a => a.City.Contains(searchString) || a.StreetName.Contains(searchString) || a.CitizenID.Contains(searchString) || a.PostalCode.Contains(searchString));
            }

            return View(await query.ToListAsync());
        }

        // GET: Address/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var address = await _context.Addresses
                .Include(a => a.Citizen)
                .FirstOrDefaultAsync(m => m.AddressID == id);

            if (address == null) return NotFound();

            if (!User.IsInRole("Admin") && address.CitizenID != User.FindFirst("CitizenID")?.Value)
            {
                return Forbid();
            }

            return View(address);
        }

        // GET: Address/Create
        public IActionResult Create()
        {
            if (User.IsInRole("Admin"))
            {
                ViewBag.CitizensList = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Citizens, "CitizenID", "CitizenName");
            }
            var model = new AddressRecord();
            if (!User.IsInRole("Admin"))
            {
                model.CitizenID = User.FindFirst("CitizenID")?.Value ?? string.Empty;
            }
            return View(model);
        }

        // POST: Address/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddressRecord address)
        {
            // Remove navigation properties from validation if necessary
            ModelState.Remove("Citizen");

            if (ModelState.IsValid)
            {
                if (!User.IsInRole("Admin") && address.CitizenID != User.FindFirst("CitizenID")?.Value)
                {
                    ModelState.AddModelError("CitizenID", $"You can only add addresses for your own Citizen ID ({User.FindFirst("CitizenID")?.Value}).");
                    return View(address);
                }

                if (!await _context.Citizens.AnyAsync(c => c.CitizenID == address.CitizenID))
                {
                    ModelState.AddModelError("CitizenID", "The specified Citizen ID does not exist.");
                    return View(address);
                }

                _context.Add(address);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            if (User.IsInRole("Admin"))
            {
                ViewBag.CitizensList = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Citizens, "CitizenID", "CitizenName", address.CitizenID);
            }
            return View(address);
        }

        // GET: Address/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var address = await _context.Addresses.FindAsync(id);
            if (address == null) return NotFound();

            ViewBag.CitizensList = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Citizens, "CitizenID", "CitizenName", address.CitizenID);
            return View(address);
        }

        // POST: Address/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, AddressRecord address)
        {
            if (id != address.AddressID) return NotFound();

            ModelState.Remove("Citizen");
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(address);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AddressExists(address.AddressID)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.CitizensList = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Citizens, "CitizenID", "CitizenName", address.CitizenID);
            return View(address);
        }

        // GET: Address/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var address = await _context.Addresses
                .Include(a => a.Citizen)
                .FirstOrDefaultAsync(m => m.AddressID == id);
            if (address == null) return NotFound();

            return View(address);
        }

        // POST: Address/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var address = await _context.Addresses.FindAsync(id);
            if (address != null)
            {
                _context.Addresses.Remove(address);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool AddressExists(int id)
        {
            return _context.Addresses.Any(e => e.AddressID == id);
        }
    }
}
