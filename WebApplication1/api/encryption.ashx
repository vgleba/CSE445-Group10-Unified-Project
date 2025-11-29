<%@ WebHandler Language="C#" Class="EncryptionHandler" %>

using System;
using System.Web;
using System.Text;
using System.IO;

public class EncryptionHandler : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";

        if (context.Request.HttpMethod != "POST")
        {
            context.Response.Write("method should be POST");
            return;
        }

        string action = context.Request.QueryString["action"];
        string data = context.Request.QueryString["data"];
        
        if (string.IsNullOrEmpty(data))
        {
            context.Response.Write("data shouldn't be null");
            return;
        }

        if (action == "encrypt")
        {
            string encrypted = Convert.ToBase64String(Encoding.UTF8.GetBytes(data));
            context.Response.Write(encrypted);
        }
        else if (action == "decrypt")
        {
            try
            {
                byte[] bytes = Convert.FromBase64String(data);
                string decrypted = Encoding.UTF8.GetString(bytes);
                context.Response.Write(decrypted);
            }
            catch
            {
                context.Response.Write("invalid base64 string");
            }
        }
        else
        {
            context.Response.Write("action should be encrypt or decrypt");
        }
    }

    public bool IsReusable { get { return false; } }
}
