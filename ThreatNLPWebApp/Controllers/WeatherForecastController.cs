using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using ThreatNLPWebApp.Models;

namespace ThreatNLPWebApp.Controllers
{
    [RoutePrefix("api")]
    public class WeatherForecastController : ApiController
    {
        private static readonly string[] Summaries =
        {
            "Freezing","Bracing","Chilly","Cool","Mild",
            "Warm","Balmy","Hot","Sweltering","Scorching"
        };

        [HttpGet]
        [Route("weather")]
        public IEnumerable<WeatherForecast> Get()
        {
            var rnd = new Random();
            return Enumerable.Range(1, 5).Select(i => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(i),
                TemperatureC = rnd.Next(-20, 55),
                Summary = Summaries[rnd.Next(Summaries.Length)]
            });
        }
    }
}
