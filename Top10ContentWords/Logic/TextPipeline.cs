using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Top10ContentWords.Utils;

namespace Top10ContentWords.Logic
{
    public static class TextPipeline
    {
        // Unicode letters + apostrophes, min length 2
        private static readonly Regex TokenRx =
            new Regex(@"[\p{L}']{2,}", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public static string[] Top10(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return new string[0];

            // If looks like HTML, strip tags (ensures tag/attribute names never count)
            string text;
            if (raw.IndexOf('<') >= 0 && raw.IndexOf('>') >= 0)
                text = HtmlToText.Convert(raw);
            else
                text = raw;

            var stops = Stopwords.Load();
            var counts = new Dictionary<string, int>(StringComparer.Ordinal);

            foreach (Match m in TokenRx.Matches(text))
            {
                var token = m.Value.ToLowerInvariant();
                if (stops.Contains(token)) continue;

                var stem = PorterStemmer.Stem(token);

                int c;
                if (!counts.TryGetValue(stem, out c)) c = 0;
                counts[stem] = c + 1;
            }

            // Order by frequency desc, then alpha
            return counts
                .OrderByDescending(kv => kv.Value)
                .ThenBy(kv => kv.Key, StringComparer.Ordinal)
                .Take(10)
                .Select(kv => kv.Key)
                .ToArray();
        }
    }
}
