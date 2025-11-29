<%@ WebHandler Language="C#" Class="WordFilterHandler" %>

using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public class WordFilterHandler : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";

        if (context.Request.HttpMethod != "POST")
        {
            context.Response.Write("method should be POST");
            return;
        }

        string text = context.Request.QueryString["text"];
        
        if (string.IsNullOrEmpty(text))
        {
            context.Response.Write("text shouldn't be null");
            return;
        }

        string filteredWords = ProcessWordFilter(text);

        context.Response.Write(filteredWords);
    }

    private string ProcessWordFilter(string str)
    {
        if (string.IsNullOrEmpty(str)) return "";

        HashSet<string> stopWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "a", "an", "and", "are", "as", "at", "be", "by", "for", "from",
            "has", "he", "in", "is", "it", "its", "of", "on", "that", "the",
            "to", "was", "were", "will", "with", "am", "been", "have", "had",
            "do", "does", "did", "can", "could", "should", "would", "may",
            "might", "must", "shall", "this", "these", "those", "they", "them",
            "their", "we", "you", "your", "i", "me", "my", "mine", "his", "her",
            "him", "she", "us", "our", "ours"
        };

        // Remove XML tags and attributes
        string clean = Regex.Replace(str, @"<[^>]*>", " ");
        clean = Regex.Replace(clean, @"\w+\s*=\s*[""'][^""']*[""']", " ");

        // Split and filter words
        var words = clean.Split(new char[] { ' ', '\t', '\n', '\r', '.', ',', '!', '?', ';', ':', '"', '\'', '(', ')', '[', ']', '{', '}' },
                               StringSplitOptions.RemoveEmptyEntries)
                        .Where(word => word.Length > 1 && 
                                      !stopWords.Contains(word) &&
                                      Regex.IsMatch(word, @"^[a-zA-Z][a-zA-Z0-9]*$"))
                        .Select(word => word.ToLower());

        return string.Join(" ", words);
    }
 
    public bool IsReusable { get { return false; } }
}
