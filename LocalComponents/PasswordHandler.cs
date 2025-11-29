using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalComponents
{
    public class PasswordHandler
    {
        public static string HashPassword(string password)
        {
            if (password == null)
                return string.Empty;

            try
            {
                // Simple hash using SHA256
                using (var sha256 = System.Security.Cryptography.SHA256.Create())
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(password);
                    byte[] hash = sha256.ComputeHash(bytes);
                    return Convert.ToBase64String(hash);
                }
            }
            catch (Exception)
            {
                return "Error during hashing";
            }
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            if (password == null || hashedPassword == null)
                return false;

            try
            {
                string hashOfInput = HashPassword(password);
                return StringComparer.Ordinal.Compare(hashOfInput, hashedPassword) == 0;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
