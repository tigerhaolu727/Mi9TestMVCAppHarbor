using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace Mi9TestMVC.Controllers
{
    public class MiJsonTestController : ApiController
    {
        public HttpResponseMessage Get()
        {
            return Request.CreateResponse(HttpStatusCode.Forbidden, JObject.FromObject(new { error = "This HttpMethod is forbidden" }));
        }

        // POST api/MiJsonTest
        public HttpResponseMessage Post()
        {
            try
            {
                string json;

                using (var reader = new StreamReader(HttpContext.Current.Request.InputStream))
                {
                    json = reader.ReadToEnd();
                }
                var data = Json.Decode(json);

                var result = new List<object>();

                if (data.payload != null)
                {
                    var payloads = data.payload;
                    foreach (var payload in payloads)
                    {
                        if (payload.drm == null || !payload.drm || payload.episodeCount == null || payload.episodeCount <= 0)
                            continue;

                        result.Add(new { image = payload.image != null ? payload.image.showImage : payload.image, slug = payload.slug, title = payload.title });
                    }
                }
                return Request.CreateResponse(HttpStatusCode.OK, new JObject(new JProperty("response", JArray.FromObject(result))));
            }
            catch (Exception ex)
            {
               var myCustomError = new HttpError{ { "error", 37 } };
               return Request.CreateErrorResponse(HttpStatusCode.BadRequest, myCustomError);
               
            }

        }
    }
}