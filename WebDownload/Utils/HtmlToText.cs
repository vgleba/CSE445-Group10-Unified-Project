using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace WebDownload.Utils;

public static class HtmlToText
{
    private static readonly Regex MultiSpace = new(@"\s+", RegexOptions.Compiled);
    private static readonly Regex BlankLines = new(@"(\r?\n)\s*(\r?\n)+", RegexOptions.Compiled);

    public static string Convert(string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        // drop scripts/styles
        var drop = doc.DocumentNode.SelectNodes("//script|//style");
        if (drop != null) foreach (var n in drop) n.Remove();

        var text = HtmlEntity.DeEntitize(doc.DocumentNode.InnerText);
        text = MultiSpace.Replace(text, " ").Trim();
        text = text.Replace("\r\n", "\n");
        text = BlankLines.Replace(text, "\n\n");
        return text.Trim();
    }
}
