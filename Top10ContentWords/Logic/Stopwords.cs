using System.Collections.Generic;
using System.IO;
using System.Web;

namespace Top10ContentWords.Logic
{
    public static class Stopwords
    {
        private static HashSet<string> _words;

        public static HashSet<string> Load()
        {
            if (_words != null) return _words;

            var mapPath = HttpContext.Current.Server.MapPath("~/App_Data/stopwords_all.txt");
            var set = new HashSet<string>();
            foreach (var line in File.ReadAllLines(mapPath))
            {
                var s = (line ?? "").Trim().ToLowerInvariant();
                if (s.Length == 0) continue;
                if (s.StartsWith("#")) continue;
                set.Add(s);
            }
            _words = set;
            return _words;
        }
    }
}
