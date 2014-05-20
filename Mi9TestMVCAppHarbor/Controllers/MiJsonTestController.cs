using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using Newtonsoft.Json.Linq;

namespace Mi9TestMVCAppHarbor.Controllers
{
    public class MiJsonTestController : ApiController
    {
        public IHttpActionResult Get()
        {
            return new BadRequestResult(this);
        }

        // POST api/MiJsonTest
        public IHttpActionResult Post()
        {
            try
            {
                string json;

                using (var reader = new StreamReader(HttpContext.Current.Request.InputStream))
                {
                    json = reader.ReadToEnd();
                }
                var data = JObject.Parse(json);

                var result = new List<object>();

                if (data["payload"] != null)
                {
                    var payloads = data["payload"];
                    foreach (var payload in payloads.Where(payload => payload["drm"] != null && payload["drm"].Value<bool>() && payload["episodeCount"] != null && payload["episodeCount"].Value<int>() > 0))
                    {
                        result.Add(new { image = payload["image"] != null ? payload["image"]["showImage"].Value<string>() : payload["image"], slug = payload["slug"].Value<string>(), title = payload["title"].Value<string>() });
                    }
                }
                return Ok(new JObject(new JProperty("response", JArray.FromObject(result))));
            }
            catch (Exception ex)
            {
                return new BadRequestResult();

                return new StatusCodeResult(HttpStatusCode.BadRequest, ModelState.AddModelError("error", "Could not decode request: JSON parsing failed"));
            }

        }
    }
}