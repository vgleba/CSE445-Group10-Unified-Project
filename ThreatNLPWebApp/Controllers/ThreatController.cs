using System.Web;
using System.Web.Http;
using ThreatNLPWebApp.Models;
using ThreatNLPWebApp.Services;

namespace ThreatNLPWebApp.Controllers
{
    [RoutePrefix("api/threat")]
    public class ThreatController : ApiController
    {
        private readonly ThreatNlpService _nlp;

        public ThreatController()
        {
            _nlp = new ThreatNlpService();
        }

        [HttpPost]
        [Route("nlp")]
        public IHttpActionResult Parse(ThreatText dto)
        {
            var events = _nlp.Parse(dto.Text ?? string.Empty, dto.Lang, dto.Ts);
            return Ok(events);
        }
    }
}
