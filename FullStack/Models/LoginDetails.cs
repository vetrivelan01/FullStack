using System.ComponentModel.DataAnnotations;

public class LoginDetails
{
    [Key]
    public string LoginID { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string LoginType { get; set; } = string.Empty;
}