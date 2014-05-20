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
        public void Get()
        {
            ErrorResponse(HttpStatusCode.Forbidden, "This HttpMethod is forbidden");
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

                var payloads = data.payload;

                var result = new List<object>();

                foreach (var payload in payloads)
                {
                    if (payload.drm == null || !payload.drm || payload.episodeCount == null || payload.episodeCount <= 0)
                        continue;
                    
                    result.Add(new { image = payload.image != null ? payload.image.showImage : payload.image, slug = payload.slug, title = payload.title });
                }

                return new JObject(new JProperty("response", JArray.FromObject(result)));
            }
            catch (Exception ex)
            {

                ErrorResponse(HttpStatusCode.BadRequest, "Could not decode request: JSON parsing failed");
                return null;
            }

        }

        public void Put()
        {
            ErrorResponse(HttpStatusCode.Forbidden, "This HttpMethod is forbidden");
        }

        public void Delete()
        {
            ErrorResponse(HttpStatusCode.Forbidden, "This HttpMethod is forbidden");
        }


        private void ErrorResponse(HttpStatusCode httpStatusCode, string error)
        {
            HttpContext.Current.Response.StatusCode = (int)httpStatusCode;

            HttpContext.Current.Response.ContentType = "application/json; charset=UTF-8";

            HttpContext.Current.Response.Write(Json.Encode(new { error = error }));

            HttpContext.Current.Response.End();
        }
    }
}