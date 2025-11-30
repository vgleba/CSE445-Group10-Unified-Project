using System.Web.Http;
using ThreatNLPWebApp.Models;
using ThreatNLPWebApp.Services;

namespace ThreatNLPWebApp.Controllers
{
    [RoutePrefix("api/threat")]
    public class GeoController : ApiController
    {
        private readonly GeoResolveService _geo;

        public GeoController()
        {
            var gaz = new GazetteerService();
            _geo = new GeoResolveService(gaz);
        }

        [HttpPost]
        [Route("georesolve")]
        public IHttpActionResult Resolve(GeoReq req)
        {
            var resp = _geo.Resolve(req);
            return Ok(resp);
        }
    }
}
