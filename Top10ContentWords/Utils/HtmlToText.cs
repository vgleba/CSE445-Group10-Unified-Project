using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace Top10ContentWords.Utils
{
    public static class HtmlToText
    {
        private static readonly Regex MultiSpace = new Regex(@"\s+", RegexOptions.Compiled);
        private static readonly Regex BlankLines = new Regex(@"(\r?\n)\s*(\r?\n)+", RegexOptions.Compiled);

        public static string Convert(string html)
        {
            if (string.IsNullOrWhiteSpace(html)) return string.Empty;

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var drop = doc.DocumentNode.SelectNodes("//script|//style");
            if (drop != null)
            {
                foreach (var n in drop) n.Remove();
            }

            var text = HtmlEntity.DeEntitize(doc.DocumentNode.InnerText ?? string.Empty);
            text = MultiSpace.Replace(text, " ");
            text = text.Replace("\r\n", "\n");
            text = BlankLines.Replace(text, "\n\n");
            return text.Trim();
        }
    }
}
