using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FullStack.Models;

public class AddressRecord
{
    [Key]
    public int AddressID { get; set; }

    [ForeignKey("Citizen")]
    public string CitizenID { get; set; } = string.Empty;

    public string HouseNumber { get; set; } = string.Empty;
    public string StreetName { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;

    public Citizen Citizen { get; set; } = null!;
}