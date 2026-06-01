using System.Collections.Generic;

namespace FullStack.Models
{
    public class DashboardViewModel
    {
        // Admin Stats
        public int TotalCitizens { get; set; }
        public int TotalDocuments { get; set; }
        public int TotalRecords { get; set; }
        public int PendingRecords { get; set; }
        public int ExpiringThisMonthCount { get; set; }
        public int ExpiredCount { get; set; }

        public Citizen CurrentCitizen { get; set; }
        public List<AddressRecord> Addresses { get; set; } = new List<AddressRecord>();
        public List<IdentityDocument> Documents { get; set; } = new List<IdentityDocument>();
        public List<CitizenRecord> Records { get; set; } = new List<CitizenRecord>();
        public List<IdentityDocument> ExpiringDocuments { get; set; } = new List<IdentityDocument>();
    }
}
