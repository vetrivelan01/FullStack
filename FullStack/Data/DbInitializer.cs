using FullStack.Models;
using FullStack.Helpers;
using System.Linq;

namespace FullStack.Data
{
    public static class DbInitializer
    {
        public static void Initialize(FullStackDbContext context)
        {
            if (!context.Logins.Any())
            {
                var logins = new LoginDetails[]
                {
                    new LoginDetails { LoginID = "admin", Password = PasswordHelper.HashPassword("password123"), LoginType = "Admin" },
                    new LoginDetails { LoginID = "testuser", Password = PasswordHelper.HashPassword("password123"), LoginType = "User" }
                };
                foreach (LoginDetails l in logins)
                {
                    context.Logins.Add(l);
                }
                context.SaveChanges();
            }

            if (!context.Citizens.Any(c => c.CitizenID == "testuser"))
            {
                var citizen = new Citizen
                {
                    CitizenID = "testuser",
                    CitizenName = "Test User",
                    Gender = "Other",
                    Nationality = "US",
                    Language = "English",
                    DateOfBirth = System.DateTime.Parse("1990-01-01"),
                    Address = "123 Test St",
                    PhoneNumber = "555-0100"
                };
                context.Citizens.Add(citizen);
                context.SaveChanges();
            }

            if (!context.Documents.Any())
            {
                var documents = new IdentityDocument[]
                {
                    new IdentityDocument
                    {
                        CitizenID = "testuser",
                        DocumentType = "Passport",
                        DocumentNumber = "P1234567",
                        IssueDate = DateTime.Now.AddYears(-9),
                        ExpiryDate = DateTime.Now.AddYears(1)
                    },
                    new IdentityDocument
                    {
                        CitizenID = "testuser",
                        DocumentType = "Driver License",
                        DocumentNumber = "DL987654",
                        IssueDate = DateTime.Now.AddYears(-4),
                        ExpiryDate = DateTime.Now.AddDays(15) // Expiring soon
                    },
                    new IdentityDocument
                    {
                        CitizenID = "testuser",
                        DocumentType = "National ID",
                        DocumentNumber = "ID112233",
                        IssueDate = DateTime.Now.AddYears(-6),
                        ExpiryDate = DateTime.Now.AddDays(-10) // Expired
                    }
                };
                foreach (IdentityDocument d in documents)
                {
                    context.Documents.Add(d);
                }
                context.SaveChanges();
            }
        }
    }
}
