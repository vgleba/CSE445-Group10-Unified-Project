using System;
using AAI_Utils;

namespace AAI_WebApp.TryIt
{
    public partial class Hash_TryIt : System.Web.UI.Page
    {
        protected void btnHash_Click(object sender, EventArgs e)
        {
            var input = txtInput.Text ?? string.Empty;
            var hash = SecurityHash.Sha256(input);
            txtHash.Text = hash;
        }
    }
}
