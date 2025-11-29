<%@ WebHandler Language="C#" Class="CatalogHandler" %>

using System;
using System.Web;
using System.Web.SessionState;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;

public class CatalogHandler : IHttpHandler, IRequiresSessionState
{
    private string catalogPath = HttpContext.Current.Server.MapPath("~/App_Data/catalog.json");

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        context.Response.StatusCode = 200;

        string method = context.Request.HttpMethod;
        string item = context.Request.QueryString["item"];
        string category = context.Request.QueryString["category"];
        string action = context.Request.QueryString["action"]; // For distinguishing between list all and get item

        if (method == "POST")
        {
            if (string.IsNullOrEmpty(item) || string.IsNullOrEmpty(category))
            {
                context.Response.Write("item and category shouldn't be null");
                return;
            }
            AddItem(category, item);
            context.Response.Write($"item {item} in category {category} was added");
        }
        else if (method == "DELETE")
        {
            if (string.IsNullOrEmpty(item) || string.IsNullOrEmpty(category))
            {
                context.Response.Write("item and category shouldn't be null");
                return;
            }
            
            bool deleted = DeleteItem(category, item);
            if (deleted)
            {
                context.Response.Write($"item {item} in category {category} was deleted");
            }
            else
            {
                context.Response.StatusCode = 404;
                context.Response.Write("category-item pair not found");
            }
        }
        else if (method == "GET")
        {
            if (action == "getitem")
            {
                if (string.IsNullOrEmpty(item) || string.IsNullOrEmpty(category))
                {
                    context.Response.Write("item and category shouldn't be null for get item operation");
                    return;
                }
                
                // Get specific item by both category and item
                var catalog = ReadCatalog();
                if (catalog.ContainsKey(category) && catalog[category] == item)
                {
                    // Store the found item in session for potential cart addition
                    context.Session["LastFoundItem"] = $"{item} - {category}";
                    context.Response.Write($"Found: Category: {category}, Item: {item}");
                }
                else
                {
                    context.Response.StatusCode = 404;
                    context.Response.Write($"Category-item pair '{category}' - '{item}' not found in catalog");
                }
            }
            else
            {
                // List all items in the catalog (default GET behavior)
                var catalog = ReadCatalog();
                if (catalog.Count == 0)
                {
                    context.Response.Write("Catalog is empty");
                }
                else
                {
                    var result = new StringBuilder();
                    result.AppendLine("Catalog Contents:");
                    foreach (var kvp in catalog)
                    {
                        result.AppendLine($"Category: {kvp.Key}, Item: {kvp.Value}");
                    }
                    context.Response.Write(result.ToString());
                }
            }
        }
        else
        {
            context.Response.Write("method should be GET, POST or DELETE");
        }
    }

    private void AddItem(string category, string item)
    {
        var catalog = ReadCatalog();
        catalog[category] = item;
        WriteCatalog(catalog);
    }

    private bool DeleteItem(string category, string item)
    {
        var catalog = ReadCatalog();
        if (catalog.ContainsKey(category) && catalog[category] == item)
        {
            catalog.Remove(category);
            WriteCatalog(catalog);
            return true;
        }
        return false;
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
        
        return JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
    }

    private void WriteCatalog(Dictionary<string, string> catalog)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(catalogPath));
        string json = JsonConvert.SerializeObject(catalog, Formatting.Indented);
        File.WriteAllText(catalogPath, json, Encoding.UTF8);
    }

    public bool IsReusable { get { return false; } }
}
