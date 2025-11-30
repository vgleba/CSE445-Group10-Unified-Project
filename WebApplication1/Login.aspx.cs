using System;
using System.Web.Security;

namespace WebApplication1
{
    public partial class Login : System.Web.UI.Page
    {
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            // Clear any previous error messages
            lblError.Text = "";

            // Get user input
            string username = txtUser.Text.Trim();
            string password = txtPass.Text;

            // Validate that both fields are provided
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                lblError.Text = "Username and password are required.";
                return;
            }

            // First check if user is staff
            if (AccountStore.ValidateStaff(username, password))
            {
                // Mark as staff member in session
                Session["IsStaff"] = true;
                FormsAuthentication.RedirectFromLoginPage(username, false);
                return;
            }

            // If not staff, check if user is member
            if (AccountStore.ValidateMember(username, password))
            {
                // Mark as regular member in session
                Session["IsStaff"] = false;
                FormsAuthentication.RedirectFromLoginPage(username, false);
                return;
            }

            // If neither staff nor member, show error
            lblError.Text = "Invalid username or password.";
        }

        protected void btnBackToDefault_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Default.aspx");
        }
    }
}