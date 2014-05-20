using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using Newtonsoft.Json.Linq;

namespace Mi9TestMVC.Controllers
{
    public class MiJsonTestController : ApiController
    {
        public JObject Get()
        {
            return ErrorResponse(HttpStatusCode.Forbidden, "Forbidden", "This HttpMethod is forbidden");
        }

        // POST api/MiJsonTest
        public JObject Post()
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

                return new JObject(new JProperty("response", JArray.FromObject(result)));
            }
            catch (Exception ex)
            {
                return ErrorResponse(HttpStatusCode.BadRequest, "Bad Request", "Could not decode request: JSON parsing failed");
            }

        }

        public JObject Put()
        {
            return ErrorResponse(HttpStatusCode.Forbidden, "Forbidden", "This HttpMethod is forbidden");
        }

        public JObject Delete()
        {
            return ErrorResponse(HttpStatusCode.Forbidden, "Forbidden", "This HttpMethod is forbidden");
        }


        private JObject ErrorResponse(HttpStatusCode httpStatusCode, string statusText, string error)
        {
            return JObject.FromObject(new { status = (int)httpStatusCode, statusText = statusText, error = error });
        }
    }
}