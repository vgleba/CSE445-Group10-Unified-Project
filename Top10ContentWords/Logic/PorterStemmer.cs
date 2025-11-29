using System.Text.RegularExpressions;

namespace Top10ContentWords.Logic
{
    public static class PorterStemmer
    {
        private static readonly Regex Step1Plural = new Regex("(sses|ies)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex Step1bEdIng = new Regex("(?<=.{3})(ing|ed)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex Step2Common = new Regex("(ational|tional|izer|ation|ator|alism|iveness|fulness|ousness|aliti|iviti|biliti)$",
                                                               RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static string Stem(string w)
        {
            if (string.IsNullOrEmpty(w) || w.Length < 3) return (w ?? string.Empty).ToLowerInvariant();

            var s = Step1Plural.Replace(w, m =>
            {
                var g = m.Groups[1].Value.ToLowerInvariant();
                if (g == "sses") return "ss";
                if (g == "ies") return "i";
                return m.Value;
            });

            s = Step1bEdIng.Replace(s, string.Empty);

            if (s.Length > 6)
            {
                s = Step2Common.Replace(s, "ate");
            }

            return s.ToLowerInvariant();
        }
    }
}
