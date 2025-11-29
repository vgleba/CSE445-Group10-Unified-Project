using System;
using System.Web.Security;

namespace WebApplication1
{
    public partial class Login : System.Web.UI.Page
    {
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            // For A5: accept any non-empty creds; real role/cred handling comes in A6.
            if (!string.IsNullOrWhiteSpace(txtUser.Text) && !string.IsNullOrWhiteSpace(txtPass.Text))
            {
                FormsAuthentication.RedirectFromLoginPage(txtUser.Text.Trim(), false);
            }
        }
    }
}