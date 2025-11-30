using System;
using System.Web.Security;

namespace WebApplication1
{
    public partial class Register : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text?.Trim();
            string password = txtPassword.Text;
            string confirmPassword = txtConfirmPassword.Text;

            // Validate input
            if (string.IsNullOrWhiteSpace(username))
            {
                litMessage.Text = "<p style='color:red;'>Username is required.</p>";
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                litMessage.Text = "<p style='color:red;'>Password is required.</p>";
                return;
            }

            if (password != confirmPassword)
            {
                litMessage.Text = "<p style='color:red;'>Passwords do not match.</p>";
                return;
            }

            // Attempt to register
            bool success = AccountStore.RegisterMember(username, password);

            if (success)
            {
                // Registration successful - mark as member (not staff) and log in the user
                Session["IsStaff"] = false;
                FormsAuthentication.SetAuthCookie(username, false);
                Response.Redirect("~/Member.aspx", false);
            }
            else
            {
                litMessage.Text = "<p style='color:red;'>Username already exists. Please choose a different username.</p>";
            }
        }
    }
}
