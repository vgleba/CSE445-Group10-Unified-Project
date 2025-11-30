using System;
using System.Data;
using System.Drawing;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Security;

namespace WebApplication1
{
    public partial class Staff : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Ensure user is authenticated
            if (!User.Identity.IsAuthenticated)
            {
                FormsAuthentication.RedirectToLoginPage();
                return;
            }

            // Ensure user is staff (has IsStaff session variable set to true)
            if (Session["IsStaff"] == null || !Convert.ToBoolean(Session["IsStaff"]))
            {
                // User is authenticated but not authorized as staff
                Response.Redirect("~/Default.aspx");
                return;
            }

            // Display welcome message with username
            if (!IsPostBack)
            {
                lblWelcome.Text = "Welcome, " + User.Identity.Name;
                BindStaffServiceDirectory();
            }
        }

        private void BindStaffServiceDirectory()
        {
            var table = new DataTable();
            table.Columns.Add("Provider");
            table.Columns.Add("Type");
            table.Columns.Add("Name");
            table.Columns.Add("Parameters");
            table.Columns.Add("Return");
            table.Columns.Add("Description");
            table.Columns.Add("TryItUrl");

            table.Rows.Add(
                "Vladyslav Saniuk",
                "REST",
                "catalog add",
                "category: string, item: string",
                "string",
                "Adds key/value to JSON catalog",
                "#tryitCatalogAdd"
            );
            table.Rows.Add(
                "Vladyslav Saniuk",
                "REST",
                "catalog delete",
                "category: string, item: string",
                "string",
                "Deletes key/value from JSON catalog",
                "#tryitCatalogDelete"
            );

            gvStaffDirectory.DataSource = table;
            gvStaffDirectory.DataBind();
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            // Sign out from Forms Authentication
            FormsAuthentication.SignOut();
            
            // Clear all session data
            Session.Clear();
            
            // Redirect to homepage
            Response.Redirect("~/Default.aspx");
        }

        protected void btnChangePassword_Click(object sender, EventArgs e)
        {
            // Ensure user is authenticated and is staff
            if (!User.Identity.IsAuthenticated ||
                Session["IsStaff"] == null || !Convert.ToBoolean(Session["IsStaff"]))
            {
                FormsAuthentication.RedirectToLoginPage();
                return;
            }

            string username = User.Identity.Name;
            string oldPassword = txtOldPassword.Text;
            string newPassword = txtNewPassword.Text;
            string confirmNewPassword = txtConfirmNewPassword.Text;

            lblChangePasswordMessage.Text = "";
            
            // Basic validation
            if (string.IsNullOrWhiteSpace(oldPassword) ||
                string.IsNullOrWhiteSpace(newPassword) ||
                string.IsNullOrWhiteSpace(confirmNewPassword))
            {
                lblChangePasswordMessage.ForeColor = Color.Red;
                lblChangePasswordMessage.Text = "All password fields are required.";
                return;
            }

            if (!string.Equals(newPassword, confirmNewPassword))
            {
                lblChangePasswordMessage.ForeColor = Color.Red;
                lblChangePasswordMessage.Text = "New password and confirmation do not match.";
                return;
            }

            // Enforce minimum password length
            if (newPassword.Length < 6)
            {
                lblChangePasswordMessage.ForeColor = Color.Red;
                lblChangePasswordMessage.Text = "New password must be at least 6 characters long.";
                return;
            }

            bool success = AccountStore.ChangeStaffPassword(username, oldPassword, newPassword);

            if (success)
            {
                lblChangePasswordMessage.ForeColor = Color.Green;
                lblChangePasswordMessage.Text = "Password changed successfully.";
                txtOldPassword.Text = "";
                txtNewPassword.Text = "";
                txtConfirmNewPassword.Text = "";
            }
            else
            {
                lblChangePasswordMessage.ForeColor = Color.Red;
                lblChangePasswordMessage.Text = "Password change failed. Current password may be incorrect.";
            }
        }

        // Catalog Management Handlers
        protected void btnCatalogAdd_Click(object sender, EventArgs e)
        {
            var baseUri = GetBaseUri();
            var url = baseUri + $"api/catalog.ashx?category={HttpUtility.UrlEncode(txtCategoryAdd.Text)}&item={HttpUtility.UrlEncode(txtItemAdd.Text)}";
            litCatalogAddResult.Text = HttpUtility.HtmlEncode(DoPost(url, ""));
        }

        protected void btnCatalogDelete_Click(object sender, EventArgs e)
        {
            var baseUri = GetBaseUri();
            var url = baseUri + $"api/catalog.ashx?category={HttpUtility.UrlEncode(txtCategoryDel.Text)}&item={HttpUtility.UrlEncode(txtItemDel.Text)}";
            litCatalogDeleteResult.Text = HttpUtility.HtmlEncode(DoDelete(url));
        }

        // Helper Methods
        private string GetBaseUri()
        {
            var req = HttpContext.Current.Request;
            var appPath = req.ApplicationPath;
            if (!appPath.EndsWith("/")) appPath += "/";
            return $"{req.Url.Scheme}://{req.Url.Authority}{appPath}";
        }

        private string DoPost(string url, string body)
        {
            using (var wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                try { return wc.UploadString(url, "POST", body ?? string.Empty); }
                catch (WebException ex) { return ReadError(ex); }
            }
        }

        private string DoGet(string url)
        {
            using (var wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                try { return wc.DownloadString(url); }
                catch (WebException ex) { return ReadError(ex); }
            }
        }

        private string DoDelete(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "DELETE";
            try
            {
                using (var resp = (HttpWebResponse)request.GetResponse())
                using (var stream = resp.GetResponseStream())
                using (var reader = new System.IO.StreamReader(stream))
                    return reader.ReadToEnd();
            }
            catch (WebException ex) { return ReadError(ex); }
        }

        private string ReadError(WebException ex)
        {
            try
            {
                using (var resp = (HttpWebResponse)ex.Response)
                using (var stream = resp.GetResponseStream())
                using (var reader = new System.IO.StreamReader(stream))
                    return reader.ReadToEnd();
            }
            catch { return ex.Message; }
        }
    }
}