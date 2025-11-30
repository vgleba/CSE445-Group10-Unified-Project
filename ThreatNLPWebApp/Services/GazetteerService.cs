using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using ThreatNLPWebApp.Models;

namespace ThreatNLPWebApp.Services
{
    public sealed class GazetteerService
    {
        private readonly string _path;
        private IReadOnlyList<GazetteerPlace> _places;
        private readonly object _lock = new object();

        public GazetteerService()
        {
            var rel = ConfigurationManager.AppSettings["GeoResolve.GazetteerPath"] ?? "App_Data/gazetteer.json";
            var basePath = HttpContext.Current.Server.MapPath("~");
            _path = Path.Combine(basePath, rel);
        }

        public IReadOnlyList<GazetteerPlace> All()
        {
            if (_places != null) return _places;
            lock (_lock)
            {
                if (_places != null) return _places;
                var json = File.ReadAllText(_path);
                var list = JsonConvert.DeserializeObject<List<GazetteerPlace>>(json) ?? new List<GazetteerPlace>();
                _places = list;
                return _places;
            }
        }

        public GazetteerPlace FindBest(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;
            var t = Normalize(text);

            GazetteerPlace best = null;
            int bestScore = -1;

            foreach (var p in All())
            {
                var cand = new[] { p.Name }.Concat(p.Aliases ?? Array.Empty<string>());
                foreach (var c in cand)
                {
                    var s = Score(Normalize(c), t);
                    if (s > bestScore) { bestScore = s; best = p; }
                }
            }

            return bestScore >= 80 ? best : null;
        }

        private static string Normalize(string s) =>
        new string(s.Trim().ToLowerInvariant().Where(ch => char.IsLetter(ch) || ch == ' ' || ch == '-').ToArray());

        // very cheap similarity: exact=100, contains=90, prefix/suffix=85 else 100 - Levenshtein%
        private static int Score(string a, string b)
        {
            if (a == b) return 100;
            if (a.Contains(b) || b.Contains(a)) return 90;
            if (a.StartsWith(b) || b.StartsWith(a) || a.EndsWith(b) || b.EndsWith(a)) return 85;
            var dist = Levenshtein(a, b);
            var max = Math.Max(a.Length, b.Length);
            var pct = (int)Math.Round(100.0 * (1.0 - (double)dist / Math.Max(1, max)));
            return pct;
        }

        private static int Levenshtein(string s, string t)
        {
            var n = s.Length; var m = t.Length;
            var d = new int[n + 1, m + 1];
            for (int i = 0; i <= n; i++) d[i, 0] = i;
            for (int j = 0; j <= m; j++) d[0, j] = j;
            for (int i = 1; i <= n; i++)
                for (int j = 1; j <= m; j++)
                {
                    var cost = s[i - 1] == t[j - 1] ? 0 : 1;
                    d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cost);
                }
            return d[n, m];
        }
    }
}
