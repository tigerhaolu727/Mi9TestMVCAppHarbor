using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
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
                HttpContext.Current.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                HttpContext.Current.Response.Write(JObject.FromObject(new { error = "Could not decode request: JSON parsing failed" }).ToString());
                HttpContext.Current.Response.AppendHeader("error", "Could not decode request: JSON parsing failed");
                HttpContext.Current.Response.ContentType = "application/json";
                HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.UTF8;
                HttpContext.Current.Response.End();

                return null;
            }

        }
    }
}