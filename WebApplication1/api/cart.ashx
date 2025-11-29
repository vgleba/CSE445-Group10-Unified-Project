<%@ WebHandler Language="C#" Class="CartHandler" %>

using System;
using System.Web;
using System.Web.SessionState;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;

public class CartHandler : IHttpHandler, IRequiresSessionState
{
    private string cartPath = HttpContext.Current.Server.MapPath("~/App_Data/cart.json");
    private string catalogPath = HttpContext.Current.Server.MapPath("~/App_Data/catalog.json");

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        context.Response.StatusCode = 200;

        string method = context.Request.HttpMethod;
        string action = context.Request.QueryString["action"];
        
        if (method == "GET")
        {
            // Refresh cart - display all items in the cart
            var cart = ReadCart();
            if (cart.Count == 0)
            {
                context.Response.Write("Cart is empty");
            }
            else
            {
                var result = new StringBuilder();
                result.AppendLine("Cart Contents:");
                int itemNumber = 1;
                foreach (var item in cart)
                {
                    result.AppendLine($"{itemNumber}. {item}");
                    itemNumber++;
                }
                context.Response.Write(result.ToString());
            }
        }
        else if (method == "POST" && action == "add")
        {
            // Try to get item data from request parameters first
            string category = context.Request.QueryString["category"];
            string item = context.Request.QueryString["item"];
            string itemToAdd = null;

            if (!string.IsNullOrEmpty(category) && !string.IsNullOrEmpty(item))
            {
                itemToAdd = $"{item} - {category}";
            }
            else
            {
                // Fallback to session data for backward compatibility
                itemToAdd = context.Session["LastFoundItem"] as string;
            }

            if (!string.IsNullOrEmpty(itemToAdd))
            {
                var cart = ReadCart();
                cart.Add(itemToAdd);
                WriteCart(cart);
                context.Response.Write($"Item '{itemToAdd}' added to cart successfully");
                // Clear the session item after adding
                context.Session["LastFoundItem"] = null;
            }
            else
            {
                context.Response.Write("No item found to add to cart. Please search for an item first.");
            }
        }
        else if (method == "POST" && action == "checkout")
        {
            // Checkout process
            var cart = ReadCart();
            if (cart.Count == 0)
            {
                context.Response.Write("Cart is empty. Nothing to checkout.");
                return;
            }

            // Show current cart contents first
            var result = new StringBuilder();
            result.AppendLine("Cart Contents:");
            int itemNumber = 1;
            foreach (var item in cart)
            {
                result.AppendLine($"{itemNumber}. {item}");
                itemNumber++;
            }
            result.AppendLine();
            result.AppendLine("Thank you for shopping with us!");

            // Remove cart items from catalog
            var catalog = ReadCatalog();
            int removedCount = 0;
            
            foreach (var cartItem in cart)
            {
                // Parse the cart item format "item - category"
                var parts = cartItem.Split(new[] { " - " }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 2)
                {
                    string item = parts[0].Trim();
                    string category = parts[1].Trim();
                    
                    // Remove from catalog if it exists
                    if (catalog.ContainsKey(category) && catalog[category] == item)
                    {
                        catalog.Remove(category);
                        removedCount++;
                    }
                }
            }

            // Save updated catalog and clear cart
            WriteCatalog(catalog);
            WriteCart(new List<string>()); // Clear the cart
            
            result.AppendLine($"({removedCount} items removed from catalog)");
            context.Response.Write(result.ToString());
        }
        else
        {
            context.Response.Write("method should be GET for cart refresh, POST with action=add for adding items, or POST with action=checkout for checkout");
        }
    }

    private List<string> ReadCart()
    {
        if (!File.Exists(cartPath))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(cartPath));
            return new List<string>();
        }
        
        string json = File.ReadAllText(cartPath, Encoding.UTF8);
        if (string.IsNullOrEmpty(json))
        {
            return new List<string>();
        }
        
        return JsonConvert.DeserializeObject<List<string>>(json) ?? new List<string>();
    }

    private void WriteCart(List<string> cart)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(cartPath));
        string json = JsonConvert.SerializeObject(cart, Formatting.Indented);
        File.WriteAllText(cartPath, json, Encoding.UTF8);
    }

    private Dictionary<string, string> ReadCatalog()
    {
        if (!File.Exists(catalogPath))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(catalogPath));
            return new Dictionary<string, string>();
        }
        
        string json = File.ReadAllText(catalogPath, Encoding.UTF8);
        if (string.IsNullOrEmpty(json))
        {
            return new Dictionary<string, string>();
        }
        
        return JsonConvert.DeserializeObject<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
    }

    private void WriteCatalog(Dictionary<string, string> catalog)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(catalogPath));
        string json = JsonConvert.SerializeObject(catalog, Formatting.Indented);
        File.WriteAllText(catalogPath, json, Encoding.UTF8);
    }

    public bool IsReusable { get { return false; } }
}