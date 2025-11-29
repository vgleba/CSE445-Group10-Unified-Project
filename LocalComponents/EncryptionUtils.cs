using System;
using System.Text;

namespace LocalComponents
{
    public static class EncryptionUtils
    {
        public static string Encrypt(string input)
        {
            if (input == null)
                return string.Empty;
                
            try
            {
                // Convert string to UTF-8 bytes, then to Base64
                byte[] bytes = Encoding.UTF8.GetBytes(input);
                return Convert.ToBase64String(bytes);
            }
            catch (Exception)
            {
                return "Error during encryption";
            }
        }

        public static string Decrypt(string base64)
        {
            if (string.IsNullOrEmpty(base64))
                return string.Empty;
                
            try
            {
                // Convert Base64 to bytes, then to UTF-8 string
                byte[] bytes = Convert.FromBase64String(base64);
                return Encoding.UTF8.GetString(bytes);
            }
            catch (Exception)
            {
                return "invalid base64 string";
            }
        }
    }
}
