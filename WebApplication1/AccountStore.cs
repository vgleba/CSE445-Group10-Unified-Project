using System;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Xml.Linq;
using LocalComponents;

namespace WebApplication1
{
    public static class AccountStore
    {
        private static readonly string MembersPath = HostingEnvironment.MapPath("~/App_Data/Members.xml");
        private static readonly string StaffPath = HostingEnvironment.MapPath("~/App_Data/Staff.xml");

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

        public static void GenerateTAAccount()
        {
            EnsureFiles();

            try
            {
                var staffDoc = XDocument.Load(StaffPath);
                
                var existingTA = staffDoc.Descendants("Staff")
                    .FirstOrDefault(s => 
                        string.Equals(s.Element("Username")?.Value, "TA", StringComparison.OrdinalIgnoreCase));

                if (existingTA == null)
                {
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
            }
        }

        public static bool ValidateStaff(string username, string plainPassword)
        {
            EnsureFiles();

            try
            {
                var staffDoc = XDocument.Load(StaffPath);

                var staffElement = staffDoc.Descendants("Staff")
                    .FirstOrDefault(s => 
                        string.Equals(s.Element("Username")?.Value, username, StringComparison.OrdinalIgnoreCase));

                if (staffElement == null)
                {
                    return false;
                }

                var storedHash = staffElement.Element("PasswordHash")?.Value;
                return PasswordHandler.VerifyPassword(plainPassword, storedHash);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool ValidateMember(string username, string plainPassword)
        {
            EnsureFiles();

            try
            {
                var membersDoc = XDocument.Load(MembersPath);

                var memberElement = membersDoc.Descendants("Member")
                    .FirstOrDefault(m => 
                        string.Equals(m.Element("Username")?.Value, username, StringComparison.OrdinalIgnoreCase));

                if (memberElement == null)
                {
                    return false;
                }

                var storedHash = memberElement.Element("PasswordHash")?.Value;
                return PasswordHandler.VerifyPassword(plainPassword, storedHash);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool RegisterMember(string username, string plainPassword)
        {
            EnsureFiles();

            try
            {
                var membersDoc = XDocument.Load(MembersPath);

                var existingMember = membersDoc.Descendants("Member")
                    .FirstOrDefault(m => 
                        string.Equals(m.Element("Username")?.Value, username, StringComparison.OrdinalIgnoreCase));

                if (existingMember != null)
                {
                    return false;
                }

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

        public static bool ChangeMemberPassword(string username, string oldPlainPassword, string newPlainPassword)
        {
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

                var memberElement = membersDoc.Descendants("Member")
                    .FirstOrDefault(m => 
                        string.Equals(m.Element("Username")?.Value, username, StringComparison.OrdinalIgnoreCase));

                if (memberElement == null)
                {
                    return false;
                }

                var storedHash = memberElement.Element("PasswordHash")?.Value;
                bool isOldPasswordValid = PasswordHandler.VerifyPassword(oldPlainPassword, storedHash);
                
                if (!isOldPasswordValid)
                {
                    return false;
                }

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

        public static bool ChangeStaffPassword(string username, string oldPlainPassword, string newPlainPassword)
        {
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

                var staffElement = staffDoc.Descendants("Staff")
                    .FirstOrDefault(s => 
                        string.Equals(s.Element("Username")?.Value, username, StringComparison.OrdinalIgnoreCase));

                if (staffElement == null)
                {
                    return false;
                }

                var storedHash = staffElement.Element("PasswordHash")?.Value;
                bool isOldPasswordValid = PasswordHandler.VerifyPassword(oldPlainPassword, storedHash);
                
                if (!isOldPasswordValid)
                {
                    return false;
                }

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
