using System;
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
    }
}