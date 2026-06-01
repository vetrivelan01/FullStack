using System;
using System.Security.Cryptography;
using System.Text;

namespace FullStack.Helpers
{
    public static class PasswordHelper
    {
        public static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        public static bool VerifyPassword(string inputPassword, string storedHash)
        {
            var hashOfInput = HashPassword(inputPassword);
            return StringComparer.OrdinalIgnoreCase.Compare(hashOfInput, storedHash) == 0;
        }
    }
}
