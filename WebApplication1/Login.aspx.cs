using System;
using System.Web.Security;

namespace WebApplication1
{
    public partial class Login : System.Web.UI.Page
    {
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            lblError.Text = "";

            string username = txtUser.Text.Trim();
            string password = txtPass.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                lblError.Text = "Username and password are required.";
                return;
            }

            if (AccountStore.ValidateStaff(username, password))
            {
                Session["IsStaff"] = true;
                FormsAuthentication.RedirectFromLoginPage(username, false);
                return;
            }

            if (AccountStore.ValidateMember(username, password))
            {
                Session["IsStaff"] = false;
                FormsAuthentication.RedirectFromLoginPage(username, false);
                return;
            }

            lblError.Text = "Invalid username or password.";
        }

        protected void btnBackToDefault_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Default.aspx");
        }
    }
}