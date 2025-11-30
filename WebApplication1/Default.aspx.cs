using LocalComponents;
using System;
using System.Data;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;
using WebApplication1.ServiceReference1;

namespace WebApplication1
{
    public partial class Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                BindServiceDirectory();
        }

        // Member / Staff navigation with auth check
        protected void btnMember_Click(object sender, EventArgs e)
        {
            NavigateProtected("~/Member.aspx");
        }

        protected void btnStaff_Click(object sender, EventArgs e)
        {
            NavigateProtected("~/Staff.aspx");
        }

        private void NavigateProtected(string protectedUrl)
        {
            if (Context.User != null && Context.User.Identity != null && Context.User.Identity.IsAuthenticated)
            {
                Response.Redirect(protectedUrl, endResponse: false);
            }
            else
            {
                var loginUrl = "~/Login.aspx";
                var returnUrl = HttpUtility.UrlEncode(VirtualPathUtility.ToAbsolute(protectedUrl));
                Response.Redirect($"{loginUrl}?returnUrl={returnUrl}", endResponse: false);
            }
        }

        // Directory rows
        private class DirectoryRow
        {
            public string Provider { get; set; }
            public string ComponentType { get; set; }
            public string Operation { get; set; }
            public string Parameters { get; set; }
            public string ReturnType { get; set; }
            public string Description { get; set; }
            public string TryItAnchor { get; set; } // #anchor in this page
        }

        private void BindServiceDirectory()
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
                "Volodymyr Gleba", 
                "REST Service",
                "ThreatNLP",
                "string text",
                "ThreatEvent[]",
                "Rule-based extractor for threat type, direction, location and timestamp.",
                "TryIt/ThreatNLP_TryIt.aspx");
            table.Rows.Add(
                "Volodymyr Gleba", 
                "REST Service",
                "WebDownload",
                "string url",
                "string plainText",
                "Downloads HTML and returns cleaned text (cap & timeout).",
                "TryIt/WebDownload_TryIt.aspx");
            table.Rows.Add(
                "Volodymyr Gleba", 
                "DLL",
                "SecurityHash.Sha256",
                "string input",
                "string hash",
                "Local hashing function DLL used for credentials and diagnostics.",
                "TryIt/Hash_TryIt.aspx");
            table.Rows.Add(
                "Volodymyr Gleba", 
                "Cookie",
                "AAI_LastThreatText",
                "",
                "string",
                "Stores last ThreatNLP input text in a browser cookie for 30 minutes.",
                "TryIt/ThreatNLP_TryIt.aspx");

            table.Rows.Add(
                "Vladyslav Saniuk",
                "WSDL (WCF)",
                "WebDownload(url: string)",
                "url: string",
                "string",
                "Fetches raw HTML/text from the URL",
                "#tryitWebDownload"
            );
            table.Rows.Add(
                "Vladyslav Saniuk",
                "REST",
                "wordfilter(text: string)",
                "text: string",
                "string (filtered words)",
                "Removes tags/stopwords; returns tokens",
                "#tryitWordFilter"
            );
            table.Rows.Add(
                "Vladyslav Saniuk",
                "REST",
                "catalog list all",
                "none",
                "string (all catalog items)",
                "Lists all category/item pairs in JSON catalog",
                "#tryitCatalogList"
            );
            table.Rows.Add(
                "Vladyslav Saniuk",
                "DLL",
                "Encrypt",
                "string input",
                "string",
                "Local component providing Base64 encryption",
                "#tryitDllEncrypt"
            );
            table.Rows.Add(
                "Vladyslav Saniuk",
                "DLL",
                "Decrypt",
                "string base64",
                "string",
                "Local component providing Base64 decryption",
                "#tryitDllDecrypt"
            );
            table.Rows.Add(
                "Dmytro Ohorodiichuk",
                "DLL",
                "Password hash",
                "string input",
                "string",
                "Hashes a password using the local DLL component",
                "#tryitDllHash"
            );
            table.Rows.Add(
                "Dmytro Ohorodiichuk",
                "DLL",
                "Password verify",
                "string input, string hash",
                "bool",
                "Verifies a password against a hash using the local DLL component",
                "#tryitDllVerify"
           );

            gvDirectory.DataSource = table;
            gvDirectory.DataBind();
        }

        // TryIt handlers
        protected void btnWebDownload_Click(object sender, EventArgs e)
        {
            try
            {
                // Exact copy from Assignment 3 webDownload_Click
                var proxy = new Service1Client();
                string url = txtUrl.Text;
                
                if (string.IsNullOrEmpty(url))
                {
                    litWebDownloadResult.Text = "Please enter a valid URL";
                    return;
                }
                
                // Ensure URL has protocol if not specified
                if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                {
                    url = "http://" + url;
                }
                
                string content = proxy.WebDownload(url);
                
                // Limit the result size for display purposes (first 2000 characters)
                if (content != null && content.Length > 2000)
                    content = content.Substring(0, 2000) + "... [Content truncated for display]";
                
                // HTML encode the content to make it safe for display
                litWebDownloadResult.Text = HttpUtility.HtmlEncode(content);
            }
            catch (Exception ex)
            {
                litWebDownloadResult.Text = "Error: " + ex.Message;
            }
        }

        protected void btnWordFilter_Click(object sender, EventArgs e)
        {
            var text = txtWordFilterInput.Text ?? "";
            var baseUri = GetBaseUri();
            var uri = baseUri + "api/wordfilter.ashx?text=" + HttpUtility.UrlEncode(text);
            litWordFilterResult.Text = HttpUtility.HtmlEncode(DoPost(uri, ""));
        }

        protected void btnCatalogList_Click(object sender, EventArgs e)
        {
            var baseUri = GetBaseUri();
            var url = baseUri + "api/catalog.ashx";
            litCatalogListResult.Text = HttpUtility.HtmlEncode(DoGet(url));
        }

        protected void btnDllEncrypt_Click(object sender, EventArgs e)
        {
            string data = txtDllEncryptInput.Text ?? "";
            try
            {
                string result = EncryptionUtils.Encrypt(data);
                litDllEncryptResult.Text = HttpUtility.HtmlEncode(result);
            }
            catch (Exception ex)
            {
                litDllEncryptResult.Text = HttpUtility.HtmlEncode("DLL encryption error: " + ex.Message);
            }
        }

        protected void btnDllDecrypt_Click(object sender, EventArgs e)
        {
            string data = txtDllDecryptInput.Text ?? "";
            try
            {
                string result = EncryptionUtils.Decrypt(data);
                litDllDecryptResult.Text = HttpUtility.HtmlEncode(result);
            }
            catch (Exception ex)
            {
                litDllDecryptResult.Text = HttpUtility.HtmlEncode("DLL decryption error: " + ex.Message);
            }
        }

        protected void btnDllHash_Click(object sender, EventArgs e)
        {
            string data = txtDllHashInput.Text ?? "";
            try
            {
                string result = PasswordHandler.HashPassword(data);
                litDllHashResult.Text = HttpUtility.HtmlEncode(result);
            }
            catch (Exception ex)
            {
                litDllHashResult.Text = HttpUtility.HtmlEncode("DLL hashing error: " + ex.ToString());
            }
        }

        protected void btnDllVerify_Click(object sender, EventArgs e)
        {
            string verifyData = txtDllVerifyInput.Text ?? "";
            string hashedData = txtDllHashedInput.Text ?? "";
            try
            {
                bool result = PasswordHandler.VerifyPassword(verifyData, hashedData);
                litDllVerifyResult.Text = HttpUtility.HtmlEncode(result);
            }
            catch (Exception ex)
            {
                litDllVerifyResult.Text = HttpUtility.HtmlEncode("DLL hashing error: " + ex.ToString());
            }
        }

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