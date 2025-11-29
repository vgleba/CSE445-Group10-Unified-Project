using AAI_Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AAI_WebApp
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                BindDirectory();
        }

        private void BindDirectory()
        {
            var table = new DataTable();
            table.Columns.Add("Provider");
            table.Columns.Add("Type");
            table.Columns.Add("Name");
            table.Columns.Add("Parameters");
            table.Columns.Add("Return");
            table.Columns.Add("Description");
            table.Columns.Add("TryItUrl");

            // Top10ContentWords
            table.Rows.Add("Volodymyr Gleba", "WCF Service",
                "Top10ContentWords",
                "string inputOrUrl",
                "List<string> top10Words",
                "Returns the top 10 content words after stopword removal and stemming.",
                "TryIt/Top10_TryIt.aspx");

            // ThreatNLP
            table.Rows.Add("Volodymyr Gleba", "REST Service",
                "ThreatNLP",
                "string text",
                "ThreatEvent[]",
                "Rule-based extractor for threat type, direction, location and timestamp.",
                "TryIt/ThreatNLP_TryIt.aspx");

            // GeoResolve
            table.Rows.Add("Volodymyr Gleba", "REST Service",
                "GeoResolve",
                "string locationText",
                "GeoPoint + radius",
                "Maps place mentions & directions to coordinates using gazetteer.",
                "TryIt/GeoResolve_TryIt.aspx");

            // WebDownload
            table.Rows.Add("Volodymyr Gleba", "REST Service",
                "WebDownload",
                "string url",
                "string plainText",
                "Downloads HTML and returns cleaned text (cap & timeout).",
                "TryIt/WebDownload_TryIt.aspx");

            // DLL Hash
            table.Rows.Add("Volodymyr Gleba", "DLL",
                "SecurityHash.Sha256",
                "string input",
                "string hash",
                "Local hashing function DLL used for credentials and diagnostics.",
                "TryIt/Hash_TryIt.aspx");

            // Cookie
            table.Rows.Add("Volodymyr Gleba", "Cookie",
                "AAI_LastThreatText",
                "",
                "string",
                "Stores last ThreatNLP input text in a browser cookie for 30 minutes.",
                "TryIt/ThreatNLP_TryIt.aspx");

            // Global.asax
            table.Rows.Add("Volodymyr Gleba", "Global.asax",
                "Application[\"ActiveSessions\"]",
                "none",
                "int",
                "Tracks number of active sessions.",
                "Default.aspx");

            gvDirectory.DataSource = table;
            gvDirectory.DataBind();
        }

        protected void btnHash_Click(object sender, EventArgs e)
        {
            var input = txtHashInput.Text;
            lblHashOutput.Text = SecurityHash.Sha256(input);
        }

        protected void btnSessions_Click(object sender, EventArgs e)
        {
            var active = Application["ActiveSessions"] ?? 0;
            lblSessions.Text = $"Active sessions: {active}";
        }
    }
}