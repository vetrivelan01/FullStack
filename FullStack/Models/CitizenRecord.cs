using System.ComponentModel.DataAnnotations;
using FullStack.Models;

public class CitizenRecord
{
    [Key]
    public int RecordID { get; set; }

    public string CitizenID { get; set; } = string.Empty;
    public int AddressID { get; set; }
    public int DocumentID { get; set; }

    public DateTime RecordDate { get; set; }
    public string RecordStatus { get; set; } = string.Empty;

    public Citizen Citizen { get; set; } = null!;
    public AddressRecord Address { get; set; } = null!;
    public IdentityDocument Document { get; set; } = null!;
}