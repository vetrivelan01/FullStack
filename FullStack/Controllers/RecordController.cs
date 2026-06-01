using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using FullStack.Models;
using System;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace FullStack.Controllers
{
    [Authorize]
    public class RecordController : Controller
    {
        private readonly FullStackDbContext _context;

        public RecordController(FullStackDbContext context)
        {
            _context = context;
        }

        // GET: Record
        public async Task<IActionResult> Index(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;
            IQueryable<CitizenRecord> query = _context.CitizenRecords;
            if (!User.IsInRole("Admin"))
            {
                var userCitizenId = User.FindFirst("CitizenID")?.Value;
                query = query.Where(r => r.CitizenID == userCitizenId);
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                query = query.Where(r => r.CitizenID.Contains(searchString) || r.RecordStatus.Contains(searchString));
            }

            return View(await query.ToListAsync());
        }

        // GET: Record/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var record = await _context.CitizenRecords
                .Include(r => r.Citizen)
                .Include(r => r.Address)
                .Include(r => r.Document)
                .FirstOrDefaultAsync(m => m.RecordID == id);

            if (record == null) return NotFound();

            if (!User.IsInRole("Admin") && record.CitizenID != User.FindFirst("CitizenID")?.Value)
            {
                return Forbid();
            }

            return View(record);
        }

        // GET: Record/Create
        public IActionResult Create()
        {
            var model = new CitizenRecord();
            if (User.IsInRole("Admin"))
            {
                ViewBag.CitizensList = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Citizens, "CitizenID", "CitizenName");
                ViewBag.AddressesList = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Addresses, "AddressID", "StreetName");
                ViewBag.DocumentsList = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Documents, "DocumentID", "DocumentType");
            }
            else
            {
                var citizenId = User.FindFirst("CitizenID")?.Value;
                model.CitizenID = citizenId;
                ViewBag.AddressesList = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Addresses.Where(a => a.CitizenID == citizenId), "AddressID", "StreetName");
                ViewBag.DocumentsList = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Documents.Where(d => d.CitizenID == citizenId), "DocumentID", "DocumentType");
            }
            return View(model);
        }

        // POST: Record/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CitizenRecord record)
        {
            // Remove navigation properties from validation if necessary
            ModelState.Remove("Citizen");
            ModelState.Remove("Address");
            ModelState.Remove("Document");

            if (ModelState.IsValid)
            {
                if (!User.IsInRole("Admin") && record.CitizenID != User.FindFirst("CitizenID")?.Value)
                {
                    ModelState.AddModelError("CitizenID", "You can only add records for your own Citizen ID.");
                    return View(record);
                }

                if (!await _context.Citizens.AnyAsync(c => c.CitizenID == record.CitizenID))
                {
                    ModelState.AddModelError("CitizenID", "The specified Citizen ID does not exist.");
                }
                if (!await _context.Addresses.AnyAsync(a => a.AddressID == record.AddressID))
                {
                    ModelState.AddModelError("AddressID", "The specified Address ID does not exist.");
                }
                if (!await _context.Documents.AnyAsync(d => d.DocumentID == record.DocumentID))
                {
                    ModelState.AddModelError("DocumentID", "The specified Document ID does not exist.");
                }

                if (!ModelState.IsValid)
                {
                    return View(record);
                }

                record.RecordDate = DateTime.Now;
                _context.Add(record);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            if (User.IsInRole("Admin"))
            {
                ViewBag.CitizensList = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Citizens, "CitizenID", "CitizenName", record.CitizenID);
                ViewBag.AddressesList = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Addresses, "AddressID", "StreetName", record.AddressID);
                ViewBag.DocumentsList = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Documents, "DocumentID", "DocumentType", record.DocumentID);
            }
            else
            {
                var citizenId = User.FindFirst("CitizenID")?.Value;
                ViewBag.AddressesList = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Addresses.Where(a => a.CitizenID == citizenId), "AddressID", "StreetName", record.AddressID);
                ViewBag.DocumentsList = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Documents.Where(d => d.CitizenID == citizenId), "DocumentID", "DocumentType", record.DocumentID);
            }

            return View(record);
        }

        // GET: Record/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var record = await _context.CitizenRecords
                .Include(r => r.Citizen)
                .Include(r => r.Address)
                .Include(r => r.Document)
                .FirstOrDefaultAsync(m => m.RecordID == id);
            if (record == null) return NotFound();

            return View(record);
        }

        // POST: Record/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var record = await _context.CitizenRecords.FindAsync(id);
            if (record != null)
            {
                _context.CitizenRecords.Remove(record);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool RecordExists(int id)
        {
            return _context.CitizenRecords.Any(e => e.RecordID == id);
        }
    }
}
