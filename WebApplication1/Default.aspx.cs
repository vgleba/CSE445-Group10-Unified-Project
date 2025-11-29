using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;
using WebApplication1.ServiceReference1;
using LocalComponents;

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
            var rows = new List<DirectoryRow>
            {
                new DirectoryRow {
                    Provider = "Vladyslav Saniuk",
                    ComponentType = "WSDL (WCF)",
                    Operation = "WebDownload(url: string)",
                    Parameters = "url: string",
                    ReturnType = "string",
                    Description = "Fetches raw HTML/text from the URL",
                    TryItAnchor = "#tryitWebDownload"
                },
                new DirectoryRow {
                    Provider = "Vladyslav Saniuk",
                    ComponentType = "REST",
                    Operation = "wordfilter(text: string)",
                    Parameters = "text: string",
                    ReturnType = "string (filtered words)",
                    Description = "Removes tags/stopwords; returns tokens",
                    TryItAnchor = "#tryitWordFilter"
                },
                new DirectoryRow {
                    Provider = "Vladyslav Saniuk",
                    ComponentType = "REST",
                    Operation = "catalog add",
                    Parameters = "category: string, item: string",
                    ReturnType = "string",
                    Description = "Adds key/value to JSON catalog",
                    TryItAnchor = "#tryitCatalogAdd"
                },
                new DirectoryRow {
                    Provider = "Vladyslav Saniuk",
                    ComponentType = "REST",
                    Operation = "catalog delete",
                    Parameters = "category: string, item: string",
                    ReturnType = "string",
                    Description = "Deletes key/value from JSON catalog",
                    TryItAnchor = "#tryitCatalogDelete"
                },
                new DirectoryRow {
                    Provider = "Vladyslav Saniuk",
                    ComponentType = "REST",
                    Operation = "catalog list all",
                    Parameters = "none",
                    ReturnType = "string (all catalog items)",
                    Description = "Lists all category/item pairs in JSON catalog",
                    TryItAnchor = "#tryitCatalogList"
                },
                new DirectoryRow {
                    Provider = "Vladyslav Saniuk",
                    ComponentType = "REST",
                    Operation = "catalog get item",
                    Parameters = "category: string, item: string",
                    ReturnType = "string (confirmation if found)",
                    Description = "Gets a specific category/item pair from JSON catalog",
                    TryItAnchor = "#tryitCatalogGet"
                },
                new DirectoryRow {
                    Provider = "Vladyslav Saniuk",
                    ComponentType = "REST",
                    Operation = "cart refresh",
                    Parameters = "none",
                    ReturnType = "string (cart items)",
                    Description = "Displays all items currently in the shopping cart",
                    TryItAnchor = "#tryitCart"
                },
                new DirectoryRow {
                    Provider = "Vladyslav Saniuk",
                    ComponentType = "REST",
                    Operation = "cart checkout",
                    Parameters = "none",
                    ReturnType = "string (thank you message)",
                    Description = "Processes checkout, shows thank you message, validate shipping address via 3rd party API and removes items from catalog",
                    TryItAnchor = "#tryitCart"
                },
                new DirectoryRow {
                    Provider = "Vladyslav Saniuk",
                    ComponentType = "DLL",
                    Operation = "Encrypt",
                    Parameters = "string input",
                    ReturnType = "string",
                    Description = "Local component providing Base64 encryption",
                    TryItAnchor = "#tryitDllEncrypt"
                },
                new DirectoryRow {
                    Provider = "Vladyslav Saniuk",
                    ComponentType = "DLL",
                    Operation = "Decrypt",
                    Parameters = "string base64",
                    ReturnType = "string",
                    Description = "Local component providing Base64 decryption",
                    TryItAnchor = "#tryitDllDecrypt"
                }
            };

            gvDirectory.DataSource = rows;
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

        protected void btnCatalogAdd_Click(object sender, EventArgs e)
        {
            var baseUri = GetBaseUri();
            var url = baseUri + $"api/catalog.ashx?category={HttpUtility.UrlEncode(txtCategoryAdd.Text)}&item={HttpUtility.UrlEncode(txtItemAdd.Text)}";
            litCatalogAddResult.Text = HttpUtility.HtmlEncode(DoPost(url, ""));
        }

        protected void btnCatalogDelete_Click(object sender, EventArgs e)
        {
            var baseUri = GetBaseUri();
            var url = baseUri + $"api/catalog.ashx?category={HttpUtility.UrlEncode(txtCategoryDel.Text)}&item={HttpUtility.UrlEncode(txtItemDel.Text)}";
            litCatalogDeleteResult.Text = HttpUtility.HtmlEncode(DoDelete(url));
        }

        protected void btnCatalogList_Click(object sender, EventArgs e)
        {
            var baseUri = GetBaseUri();
            var url = baseUri + "api/catalog.ashx";
            litCatalogListResult.Text = HttpUtility.HtmlEncode(DoGet(url));
        }

        protected void btnCatalogGet_Click(object sender, EventArgs e)
        {
            var baseUri = GetBaseUri();
            var url = baseUri + $"api/catalog.ashx?action=getitem&category={HttpUtility.UrlEncode(txtCategoryGet.Text)}&item={HttpUtility.UrlEncode(txtItemGet.Text)}";
            var result = DoGet(url);
            litCatalogGetResult.Text = HttpUtility.HtmlEncode(result);
            
            // Show the "Add to Cart" button only if the item was found
            if (result.StartsWith("Found:"))
            {
                btnAddToCart.Visible = true;
                // Store the found item data in ViewState for the Add to Cart operation
                ViewState["FoundCategory"] = txtCategoryGet.Text;
                ViewState["FoundItem"] = txtItemGet.Text;
            }
            else
            {
                btnAddToCart.Visible = false;
                ViewState["FoundCategory"] = null;
                ViewState["FoundItem"] = null;
            }
        }

        protected void btnAddToCart_Click(object sender, EventArgs e)
        {
            var category = ViewState["FoundCategory"] as string;
            var item = ViewState["FoundItem"] as string;
            
            if (!string.IsNullOrEmpty(category) && !string.IsNullOrEmpty(item))
            {
                var baseUri = GetBaseUri();
                var url = baseUri + $"api/cart.ashx?action=add&category={HttpUtility.UrlEncode(category)}&item={HttpUtility.UrlEncode(item)}";
                var result = DoPost(url, "");
                
                // Show the result and hide the Add to Cart button
                litCatalogGetResult.Text += Environment.NewLine + Environment.NewLine + HttpUtility.HtmlEncode(result);
                btnAddToCart.Visible = false;
                
                // Clear the ViewState
                ViewState["FoundCategory"] = null;
                ViewState["FoundItem"] = null;
            }
            else
            {
                litCatalogGetResult.Text += Environment.NewLine + Environment.NewLine + "Error: No item data available to add to cart.";
                btnAddToCart.Visible = false;
            }
        }

        protected void btnCartRefresh_Click(object sender, EventArgs e)
        {
            var baseUri = GetBaseUri();
            var url = baseUri + "api/cart.ashx";
            litCartResult.Text = HttpUtility.HtmlEncode(DoGet(url));
        }

        protected void btnCartCheckout_Click(object sender, EventArgs e)
        {
            var baseUri = GetBaseUri();
            var url = baseUri + "api/cart.ashx?action=checkout";
            var result = DoPost(url, "");
            litCartResult.Text = HttpUtility.HtmlEncode(result);
            
            // Show the address validation panel after displaying cart contents
            if (result.Contains("Thank you for shopping with us!"))
            {
                pnlAddress.Visible = true;
                lblAddressError.Text = ""; // Clear any previous error
            }
        }

        protected void btnProceed_Click(object sender, EventArgs e)
        {
            try
            {
                // Read user inputs
                string userState = txtState.Text?.Trim();
                string userZip = txtZip.Text?.Trim();
                
                if (string.IsNullOrEmpty(userState) || string.IsNullOrEmpty(userZip))
                {
                    lblAddressError.Text = "Please provide both State and ZIP code.";
                    return;
                }

                // Call zippopotam.us API to validate ZIP code and state
                string apiUrl = $"http://api.zippopotam.us/us/{userZip}";
                
                using (var wc = new WebClient())
                {
                    wc.Encoding = Encoding.UTF8;
                    string jsonResponse = wc.DownloadString(apiUrl);
                    
                    // Parse JSON response using basic string parsing (since we're avoiding external JSON libraries)
                    // Look for the state information in the JSON response
                    string apiState = ExtractStateFromZipResponse(jsonResponse);
                    
                    if (string.IsNullOrEmpty(apiState))
                    {
                        lblAddressError.Text = "Invalid ZIP code. Please provide a valid ZIP code.";
                        return;
                    }
                    
                    // Compare states (case-insensitive)
                    if (string.Equals(userState, apiState, StringComparison.OrdinalIgnoreCase) ||
                        IsStateAbbreviationMatch(userState, apiState))
                    {
                        // Valid address - show success and hide panel
                        litCartResult.Text += "\n\nOrder processed successfully! Your items will be shipped to " + 
                                            userState + ", " + userZip + ".";
                        pnlAddress.Visible = false;
                    }
                    else
                    {
                        lblAddressError.Text = "Invalid address. The provided state does not match the ZIP code. Please provide a valid State and ZIP code.";
                    }
                }
            }
            catch (WebException webEx)
            {
                // Handle API errors (404, network issues, etc.)
                lblAddressError.Text = $"Invalid address. Please provide a valid State and ZIP code. (Error: {webEx.Message})";
            }
            catch (Exception ex)
            {
                // Handle other errors
                lblAddressError.Text = $"Error validating address: {ex.Message}. Please try again.";
            }
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

        // Helper method to extract state from ZIP API response
        private string ExtractStateFromZipResponse(string jsonResponse)
        {
            try
            {
                // Simple JSON parsing to extract state from zippopotam.us response
                // Look for "places":[{"place name":"...","state":"...","state abbreviation":"..."}]
                
                // Find the state field
                int stateIndex = jsonResponse.IndexOf("\"state\":");
                if (stateIndex > 0)
                {
                    int startQuote = jsonResponse.IndexOf("\"", stateIndex + 8);
                    int endQuote = jsonResponse.IndexOf("\"", startQuote + 1);
                    if (startQuote > 0 && endQuote > startQuote)
                    {
                        return jsonResponse.Substring(startQuote + 1, endQuote - startQuote - 1);
                    }
                }
                
                return null;
            }
            catch
            {
                return null;
            }
        }

        // Helper method to check if user provided state abbreviation matches full state name
        private bool IsStateAbbreviationMatch(string userInput, string fullStateName)
        {
            // Simple mapping for common states - in a real app you'd have a complete mapping
            var stateAbbreviations = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                {"AL", "Alabama"}, {"AK", "Alaska"}, {"AZ", "Arizona"}, {"AR", "Arkansas"}, {"CA", "California"},
                {"CO", "Colorado"}, {"CT", "Connecticut"}, {"DE", "Delaware"}, {"FL", "Florida"}, {"GA", "Georgia"},
                {"HI", "Hawaii"}, {"ID", "Idaho"}, {"IL", "Illinois"}, {"IN", "Indiana"}, {"IA", "Iowa"},
                {"KS", "Kansas"}, {"KY", "Kentucky"}, {"LA", "Louisiana"}, {"ME", "Maine"}, {"MD", "Maryland"},
                {"MA", "Massachusetts"}, {"MI", "Michigan"}, {"MN", "Minnesota"}, {"MS", "Mississippi"}, {"MO", "Missouri"},
                {"MT", "Montana"}, {"NE", "Nebraska"}, {"NV", "Nevada"}, {"NH", "New Hampshire"}, {"NJ", "New Jersey"},
                {"NM", "New Mexico"}, {"NY", "New York"}, {"NC", "North Carolina"}, {"ND", "North Dakota"}, {"OH", "Ohio"},
                {"OK", "Oklahoma"}, {"OR", "Oregon"}, {"PA", "Pennsylvania"}, {"RI", "Rhode Island"}, {"SC", "South Carolina"},
                {"SD", "South Dakota"}, {"TN", "Tennessee"}, {"TX", "Texas"}, {"UT", "Utah"}, {"VT", "Vermont"},
                {"VA", "Virginia"}, {"WA", "Washington"}, {"WV", "West Virginia"}, {"WI", "Wisconsin"}, {"WY", "Wyoming"}
            };

            // Check if user input is an abbreviation that matches the full state name
            if (stateAbbreviations.ContainsKey(userInput) && 
                string.Equals(stateAbbreviations[userInput], fullStateName, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            
            // Check if full state name matches abbreviation lookup (reverse check)
            foreach (var kvp in stateAbbreviations)
            {
                if (string.Equals(kvp.Value, fullStateName, StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(kvp.Key, userInput, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            
            return false;
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

        private string DoDelete(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "DELETE";
            try
            {
                using (var resp = (HttpWebResponse)request.GetResponse())
                using (var stream = resp.GetResponseStream())
                using (var reader = new System.IO.StreamReader(stream))
                    return reader.ReadToEnd();
            }
            catch (WebException ex) { return ReadError(ex); }
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