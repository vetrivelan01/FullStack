using FullStack.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace FullStack.Controllers
{
    public class HomeController : Controller
    {
        private readonly FullStackDbContext _context;

        public HomeController(FullStackDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var model = new DashboardViewModel();

            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Admin"))
                {
                    model.TotalCitizens = await _context.Citizens.CountAsync();
                    model.TotalDocuments = await _context.Documents.CountAsync();
                    model.TotalRecords = await _context.CitizenRecords.CountAsync();
                    model.PendingRecords = await _context.CitizenRecords.CountAsync(r => r.RecordStatus == "Pending");

                    // Document Expiry Stats
                    var today = DateTime.Today;
                    var thirtyDaysFromNow = today.AddDays(30);
                    model.ExpiringThisMonthCount = await _context.Documents.CountAsync(d => d.ExpiryDate >= today && d.ExpiryDate <= thirtyDaysFromNow);
                    model.ExpiredCount = await _context.Documents.CountAsync(d => d.ExpiryDate < today);
                    model.ExpiringDocuments = await _context.Documents
                        .Include(d => d.Citizen)
                        .Where(d => d.ExpiryDate < thirtyDaysFromNow)
                        .OrderBy(d => d.ExpiryDate)
                        .Take(10)
                        .ToListAsync();
                }
                else
                {
                    var citizenId = User.FindFirst("CitizenID")?.Value;
                    model.CurrentCitizen = await _context.Citizens.FirstOrDefaultAsync(c => c.CitizenID == citizenId);

                    if (model.CurrentCitizen != null)
                    {
                        model.Addresses = await _context.Addresses.Where(a => a.CitizenID == citizenId).ToListAsync();
                        model.Documents = await _context.Documents.Where(d => d.CitizenID == citizenId).ToListAsync();
                        model.Records = await _context.CitizenRecords.Where(r => r.CitizenID == citizenId).ToListAsync();

                        // Personal Expiry Stats
                        var today = DateTime.Today;
                        var thirtyDaysFromNow = today.AddDays(30);
                        model.ExpiringThisMonthCount = model.Documents.Count(d => d.ExpiryDate >= today && d.ExpiryDate <= thirtyDaysFromNow);
                        model.ExpiredCount = model.Documents.Count(d => d.ExpiryDate < today);
                        model.ExpiringDocuments = model.Documents
                            .Where(d => d.ExpiryDate < thirtyDaysFromNow)
                            .OrderBy(d => d.ExpiryDate)
                            .ToList();
                    }
                }
            }

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
