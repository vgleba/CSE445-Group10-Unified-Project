using System.Security.Cryptography;
using System.Text;

namespace AAI_Utils
{
    public static class SecurityHash
    {
        public static string Sha256(string input)
        {
            if (input == null) return string.Empty;

            using (var sha = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(input);
                var hash = sha.ComputeHash(bytes);
                var sb = new StringBuilder();
                foreach (var b in hash)
                    sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }
    }
}
