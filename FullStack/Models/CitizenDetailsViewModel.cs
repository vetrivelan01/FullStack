using System.Collections.Generic;

namespace FullStack.Models
{
    public class CitizenDetailsViewModel
    {
        public Citizen Citizen { get; set; }
        public List<AddressRecord> Addresses { get; set; } = new List<AddressRecord>();
        public List<IdentityDocument> Documents { get; set; } = new List<IdentityDocument>();
        public List<CitizenRecord> Records { get; set; } = new List<CitizenRecord>();
    }
}
