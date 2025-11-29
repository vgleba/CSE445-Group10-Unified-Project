using System;
using AAI_WebApp.Top10Svc;

namespace AAI_WebApp.TryIt
{
    public partial class Top10_TryIt : System.Web.UI.Page
    {
        protected void btnCall_OnClick(object sender, EventArgs e)
        {
            var client = new TextAnalyticsClient();
            var input = txtInput.Text;
            var words = client.Top10ContentWords(input);

            bltResults.Items.Clear();
            foreach (var w in words)
            {
                bltResults.Items.Add(w);
            }
        }
    }
}
