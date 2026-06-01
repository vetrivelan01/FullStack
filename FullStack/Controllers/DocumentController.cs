using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using FullStack.Models;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace FullStack.Controllers
{
    [Authorize]
    public class DocumentController : Controller
    {
        private readonly FullStackDbContext _context;

        public DocumentController(FullStackDbContext context)
        {
            _context = context;
        }

        // GET: Document
        public async Task<IActionResult> Index(string searchString, string expiryFilter, DateTime? startDate, DateTime? endDate, int? pageNumber)
        {
            ViewData["CurrentFilter"] = searchString;
            ViewData["ExpiryFilter"] = expiryFilter;
            ViewData["StartDate"] = startDate?.ToString("yyyy-MM-dd");
            ViewData["EndDate"] = endDate?.ToString("yyyy-MM-dd");

            IQueryable<IdentityDocument> query = _context.Documents.Include(d => d.Citizen);
            if (!User.IsInRole("Admin"))
            {
                var userCitizenId = User.FindFirst("CitizenID")?.Value;
                query = query.Where(d => d.CitizenID == userCitizenId);
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                query = query.Where(d => d.DocumentID.ToString() == searchString || d.CitizenID.Contains(searchString) || d.DocumentType.Contains(searchString));
            }

            // Expiry Filtering
            var today = DateTime.Today;
            if (!string.IsNullOrEmpty(expiryFilter))
            {
                switch (expiryFilter)
                {
                    case "7Days":
                        query = query.Where(d => d.ExpiryDate >= today && d.ExpiryDate <= today.AddDays(7));
                        break;
                    case "30Days":
                        query = query.Where(d => d.ExpiryDate >= today && d.ExpiryDate <= today.AddDays(30));
                        break;
                    case "Expired":
                        query = query.Where(d => d.ExpiryDate < today);
                        break;
                    case "Custom":
                        if (startDate.HasValue) query = query.Where(d => d.ExpiryDate >= startDate.Value);
                        if (endDate.HasValue) query = query.Where(d => d.ExpiryDate <= endDate.Value);
                        break;
                }
            }

            int pageSize = 10;
            return View(await Helpers.PaginatedList<IdentityDocument>.CreateAsync(query.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: Document/ExportCSV
        public async Task<IActionResult> ExportCSV(string searchString, string expiryFilter, DateTime? startDate, DateTime? endDate)
        {
            IQueryable<IdentityDocument> query = _context.Documents.Include(d => d.Citizen);
            if (!User.IsInRole("Admin"))
            {
                var userCitizenId = User.FindFirst("CitizenID")?.Value;
                query = query.Where(d => d.CitizenID == userCitizenId);
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                query = query.Where(d => d.DocumentID.ToString() == searchString || d.CitizenID.Contains(searchString) || d.DocumentType.Contains(searchString));
            }

            var today = DateTime.Today;
            if (!string.IsNullOrEmpty(expiryFilter))
            {
                switch (expiryFilter)
                {
                    case "7Days":
                        query = query.Where(d => d.ExpiryDate >= today && d.ExpiryDate <= today.AddDays(7));
                        break;
                    case "30Days":
                        query = query.Where(d => d.ExpiryDate >= today && d.ExpiryDate <= today.AddDays(30));
                        break;
                    case "Expired":
                        query = query.Where(d => d.ExpiryDate < today);
                        break;
                    case "Custom":
                        if (startDate.HasValue) query = query.Where(d => d.ExpiryDate >= startDate.Value);
                        if (endDate.HasValue) query = query.Where(d => d.ExpiryDate <= endDate.Value);
                        break;
                }
            }

            var documents = await query.ToListAsync();

            var csv = new System.Text.StringBuilder();
            // Add UTF-8 BOM for Excel compatibility
            csv.Append('\uFEFF');
            csv.AppendLine("Citizen Name,Document Type,Document Number,Issue Date,Expiry Date,Status");

            foreach (var doc in documents)
            {
                string status = doc.ExpiryDate < today ? "Expired" : (doc.ExpiryDate <= today.AddDays(30) ? "Expiring Soon" : "Valid");
                csv.AppendLine($"\"{doc.Citizen?.CitizenName}\",\"{doc.DocumentType}\",\"{doc.DocumentNumber}\",\"{doc.IssueDate:yyyy-MM-dd}\",\"{doc.ExpiryDate:yyyy-MM-dd}\",\"{status}\"");
            }

            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(csv.ToString());
            return File(buffer, "text/csv", $"Documents_Report_{DateTime.Now:yyyyMMdd}.csv");
        }

        // GET: Document/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var document = await _context.Documents
                .Include(d => d.Citizen)
                .FirstOrDefaultAsync(m => m.DocumentID == id);

            if (document == null) return NotFound();

            if (!User.IsInRole("Admin") && document.CitizenID != User.FindFirst("CitizenID")?.Value)
            {
                return Forbid();
            }

            return View(document);
        }

        // GET: Document/Create
        public IActionResult Create()
        {
            if (User.IsInRole("Admin"))
            {
                ViewBag.CitizensList = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Citizens, "CitizenID", "CitizenName");
            }
            var model = new IdentityDocument();
            model.IssueDate = DateTime.Today; // Set default date to current date
            if (!User.IsInRole("Admin"))
            {
                model.CitizenID = User.FindFirst("CitizenID")?.Value;
            }
            return View(model);
        }

        // POST: Document/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IdentityDocument document)
        {
            // Remove navigation properties from validation if necessary
            ModelState.Remove("Citizen");

            if (ModelState.IsValid)
            {
                if (!User.IsInRole("Admin") && document.CitizenID != User.FindFirst("CitizenID")?.Value)
                {
                    ModelState.AddModelError("CitizenID", "You can only add documents for your own Citizen ID.");
                    return View(document);
                }

                if (!await _context.Citizens.AnyAsync(c => c.CitizenID == document.CitizenID))
                {
                    ModelState.AddModelError("CitizenID", "The specified Citizen ID does not exist.");
                    return View(document);
                }

                document.ExpiryDate = document.IssueDate.AddYears(5);
                _context.Add(document);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            if (User.IsInRole("Admin"))
            {
                ViewBag.CitizensList = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Citizens, "CitizenID", "CitizenName", document.CitizenID);
            }
            return View(document);
        }

        // GET: Document/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var document = await _context.Documents.FindAsync(id);
            if (document == null) return NotFound();

            ViewBag.CitizensList = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Citizens, "CitizenID", "CitizenName", document.CitizenID);
            return View(document);
        }

        // POST: Document/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, IdentityDocument document)
        {
            if (id != document.DocumentID) return NotFound();

            ModelState.Remove("Citizen");
            if (ModelState.IsValid)
            {
                try
                {
                    document.ExpiryDate = document.IssueDate.AddYears(5);
                    _context.Update(document);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DocumentExists(document.DocumentID)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.CitizensList = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Citizens, "CitizenID", "CitizenName", document.CitizenID);
            return View(document);
        }

        // GET: Document/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var document = await _context.Documents
                .Include(d => d.Citizen)
                .FirstOrDefaultAsync(m => m.DocumentID == id);
            if (document == null) return NotFound();

            return View(document);
        }

        // POST: Document/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var document = await _context.Documents.FindAsync(id);
            if (document != null)
            {
                _context.Documents.Remove(document);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool DocumentExists(int id)
        {
            return _context.Documents.Any(e => e.DocumentID == id);
        }
    }
}
