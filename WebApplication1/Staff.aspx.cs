using System;
using System.Drawing;
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
            }
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
    }
}