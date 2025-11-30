using System;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Xml.Linq;
using LocalComponents;

namespace WebApplication1
{
    /// <summary>
    /// Static helper class for managing XML-based account storage with hashed passwords
    /// </summary>
    public static class AccountStore
    {
        private static readonly string MembersPath = HostingEnvironment.MapPath("~/App_Data/Members.xml");
        private static readonly string StaffPath = HostingEnvironment.MapPath("~/App_Data/Staff.xml");

        /// <summary>
        /// Ensures XML files and App_Data folder exist, creating them if necessary
        /// </summary>
        private static void EnsureFiles()
        {
            string appDataDir = HostingEnvironment.MapPath("~/App_Data");
            if (!Directory.Exists(appDataDir))
            {
                Directory.CreateDirectory(appDataDir);
            }

            if (!File.Exists(MembersPath))
            {
                var membersDoc = new XDocument(new XElement("Members"));
                membersDoc.Save(MembersPath);
            }

            if (!File.Exists(StaffPath))
            {
                var staffDoc = new XDocument(new XElement("StaffList"));
                staffDoc.Save(StaffPath);
            }
        }

        /// <summary>
        /// Generates TA staff account if it doesn't exist. Called on application startup.
        /// </summary>
        public static void GenerateTAAccount()
        {
            EnsureFiles();

            try
            {
                var staffDoc = XDocument.Load(StaffPath);
                
                // Check if TA account already exists (case-insensitive)
                var existingTA = staffDoc.Descendants("Staff")
                    .FirstOrDefault(s => 
                        string.Equals(s.Element("Username")?.Value, "TA", StringComparison.OrdinalIgnoreCase));

                if (existingTA == null)
                {
                    // Create TA account with password "Cse445!" using proper hashing
                    var hashedPassword = PasswordHandler.HashPassword("Cse445!");
                    var taStaff = new XElement("Staff",
                        new XElement("Username", "TA"),
                        new XElement("PasswordHash", hashedPassword)
                    );

                    staffDoc.Root.Add(taStaff);
                    staffDoc.Save(StaffPath);
                }
            }
            catch (Exception)
            {
                // Silently fail - staff file may be locked or inaccessible
            }
        }

        /// <summary>
        /// Validates staff credentials against Staff.xml
        /// </summary>
        /// <param name="username">Username to validate (case-insensitive)</param>
        /// <param name="plainPassword">Plain text password to check</param>
        /// <returns>True if credentials are valid, false otherwise</returns>
        public static bool ValidateStaff(string username, string plainPassword)
        {
            EnsureFiles();

            try
            {
                var staffDoc = XDocument.Load(StaffPath);

                // Find staff with matching username (case-insensitive)
                var staffElement = staffDoc.Descendants("Staff")
                    .FirstOrDefault(s => 
                        string.Equals(s.Element("Username")?.Value, username, StringComparison.OrdinalIgnoreCase));

                if (staffElement == null)
                {
                    return false;
                }

                // Get stored hash and verify password
                var storedHash = staffElement.Element("PasswordHash")?.Value;
                return PasswordHandler.VerifyPassword(plainPassword, storedHash);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Validates member credentials against Members.xml
        /// </summary>
        /// <param name="username">Username to validate (case-insensitive)</param>
        /// <param name="plainPassword">Plain text password to check</param>
        /// <returns>True if credentials are valid, false otherwise</returns>
        public static bool ValidateMember(string username, string plainPassword)
        {
            EnsureFiles();

            try
            {
                var membersDoc = XDocument.Load(MembersPath);

                // Find member with matching username (case-insensitive)
                var memberElement = membersDoc.Descendants("Member")
                    .FirstOrDefault(m => 
                        string.Equals(m.Element("Username")?.Value, username, StringComparison.OrdinalIgnoreCase));

                if (memberElement == null)
                {
                    return false;
                }

                // Get stored hash and verify password
                var storedHash = memberElement.Element("PasswordHash")?.Value;
                return PasswordHandler.VerifyPassword(plainPassword, storedHash);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Registers a new member account
        /// </summary>
        /// <param name="username">Username for new account (case-insensitive check for duplicates)</param>
        /// <param name="plainPassword">Plain text password to hash and store</param>
        /// <returns>True if registration successful, false if username already exists</returns>
        public static bool RegisterMember(string username, string plainPassword)
        {
            EnsureFiles();

            try
            {
                var membersDoc = XDocument.Load(MembersPath);

                // Check if username already exists (case-insensitive)
                var existingMember = membersDoc.Descendants("Member")
                    .FirstOrDefault(m => 
                        string.Equals(m.Element("Username")?.Value, username, StringComparison.OrdinalIgnoreCase));

                if (existingMember != null)
                {
                    return false; // Username already exists
                }

                // Add new member with hashed password
                var hashedPassword = PasswordHandler.HashPassword(plainPassword);
                var newMember = new XElement("Member",
                    new XElement("Username", username),
                    new XElement("PasswordHash", hashedPassword)
                );

                membersDoc.Root.Add(newMember);
                membersDoc.Save(MembersPath);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Changes password for an existing member account
        /// </summary>
        /// <param name="username">Username of member account (case-insensitive)</param>
        /// <param name="oldPlainPassword">Current plain text password for verification</param>
        /// <param name="newPlainPassword">New plain text password to set</param>
        /// <returns>True if password changed successfully, false if current password is wrong or user not found</returns>
        public static bool ChangeMemberPassword(string username, string oldPlainPassword, string newPlainPassword)
        {
            // Validate input parameters
            if (string.IsNullOrWhiteSpace(username) || 
                string.IsNullOrWhiteSpace(oldPlainPassword) || 
                string.IsNullOrWhiteSpace(newPlainPassword))
            {
                return false;
            }

            EnsureFiles();

            try
            {
                var membersDoc = XDocument.Load(MembersPath);

                // Find member with matching username (case-insensitive)
                var memberElement = membersDoc.Descendants("Member")
                    .FirstOrDefault(m => 
                        string.Equals(m.Element("Username")?.Value, username, StringComparison.OrdinalIgnoreCase));

                if (memberElement == null)
                {
                    return false; // Member not found
                }

                // Verify old password
                var storedHash = memberElement.Element("PasswordHash")?.Value;
                bool isOldPasswordValid = PasswordHandler.VerifyPassword(oldPlainPassword, storedHash);
                
                if (!isOldPasswordValid)
                {
                    return false; // Current password is incorrect
                }

                // Hash new password and update
                var newPasswordHash = PasswordHandler.HashPassword(newPlainPassword);
                memberElement.Element("PasswordHash").Value = newPasswordHash;
                membersDoc.Save(MembersPath);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Changes password for an existing staff account
        /// </summary>
        /// <param name="username">Username of staff account (case-insensitive)</param>
        /// <param name="oldPlainPassword">Current plain text password for verification</param>
        /// <param name="newPlainPassword">New plain text password to set</param>
        /// <returns>True if password changed successfully, false if current password is wrong or user not found</returns>
        public static bool ChangeStaffPassword(string username, string oldPlainPassword, string newPlainPassword)
        {
            // Validate input parameters
            if (string.IsNullOrWhiteSpace(username) || 
                string.IsNullOrWhiteSpace(oldPlainPassword) || 
                string.IsNullOrWhiteSpace(newPlainPassword))
            {
                return false;
            }

            EnsureFiles();

            try
            {
                var staffDoc = XDocument.Load(StaffPath);

                // Find staff with matching username (case-insensitive)
                var staffElement = staffDoc.Descendants("Staff")
                    .FirstOrDefault(s => 
                        string.Equals(s.Element("Username")?.Value, username, StringComparison.OrdinalIgnoreCase));

                if (staffElement == null)
                {
                    return false; // Staff not found
                }

                // Verify old password
                var storedHash = staffElement.Element("PasswordHash")?.Value;
                bool isOldPasswordValid = PasswordHandler.VerifyPassword(oldPlainPassword, storedHash);
                
                if (!isOldPasswordValid)
                {
                    return false; // Current password is incorrect
                }

                // Hash new password and update
                var newPasswordHash = PasswordHandler.HashPassword(newPlainPassword);
                staffElement.Element("PasswordHash").Value = newPasswordHash;
                staffDoc.Save(StaffPath);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
