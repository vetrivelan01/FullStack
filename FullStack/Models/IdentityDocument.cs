using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FullStack.Models;

public class IdentityDocument
{
    [Key]
    public int DocumentID { get; set; }

    [ForeignKey("Citizen")]
    public string CitizenID { get; set; } = string.Empty;

    public string DocumentType { get; set; } = string.Empty;
    public string DocumentNumber { get; set; } = string.Empty;
    public DateTime ExpiryDate { get; set; }
    public DateTime IssueDate { get; set; }

    public Citizen Citizen { get; set; } = null!;
}