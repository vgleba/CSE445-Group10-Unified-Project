using System;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Xml.Linq;
using LocalComponents;

namespace WebApplication1
{
    /// <summary>
    /// Static helper class for managing XML-based account storage with encrypted passwords
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
                    // Create TA account with password "Cse445!"
                    var encryptedPassword = EncryptionUtils.Encrypt("Cse445!");
                    var taStaff = new XElement("Staff",
                        new XElement("Username", "TA"),
                        new XElement("PasswordHash", encryptedPassword)
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
                var encryptedPassword = EncryptionUtils.Encrypt(plainPassword);

                // Find staff with matching username and password (case-insensitive username)
                var matchingStaff = staffDoc.Descendants("Staff")
                    .FirstOrDefault(s => 
                        string.Equals(s.Element("Username")?.Value, username, StringComparison.OrdinalIgnoreCase) &&
                        s.Element("PasswordHash")?.Value == encryptedPassword);

                return matchingStaff != null;
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
                var encryptedPassword = EncryptionUtils.Encrypt(plainPassword);

                // Find member with matching username and password (case-insensitive username)
                var matchingMember = membersDoc.Descendants("Member")
                    .FirstOrDefault(m => 
                        string.Equals(m.Element("Username")?.Value, username, StringComparison.OrdinalIgnoreCase) &&
                        m.Element("PasswordHash")?.Value == encryptedPassword);

                return matchingMember != null;
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
        /// <param name="plainPassword">Plain text password to encrypt and store</param>
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

                // Add new member with encrypted password
                var encryptedPassword = EncryptionUtils.Encrypt(plainPassword);
                var newMember = new XElement("Member",
                    new XElement("Username", username),
                    new XElement("PasswordHash", encryptedPassword)
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
                var encryptedOldPassword = EncryptionUtils.Encrypt(oldPlainPassword);
                var encryptedNewPassword = EncryptionUtils.Encrypt(newPlainPassword);

                // Find member with matching username and current password
                var memberElement = membersDoc.Descendants("Member")
                    .FirstOrDefault(m => 
                        string.Equals(m.Element("Username")?.Value, username, StringComparison.OrdinalIgnoreCase) &&
                        m.Element("PasswordHash")?.Value == encryptedOldPassword);

                if (memberElement == null)
                {
                    return false; // Member not found or current password is incorrect
                }

                // Update password hash with new encrypted password
                memberElement.Element("PasswordHash").Value = encryptedNewPassword;
                membersDoc.Save(MembersPath);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
