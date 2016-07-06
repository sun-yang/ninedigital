using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace NineDigitalWebService.Controllers
{   
    public class NineDigitalController : ApiController
    {
        [HttpPost]
        public async Task<HttpResponseMessage> JsonFilter()
        {
            // Verify that this is JSON content
            if (!IsJsonContent(Request.Content))
            {
                return Request.CreateResponse(HttpStatusCode.UnsupportedMediaType);
            }

            try
            {
                // Parse JSON
                var content = await Request.Content.ReadAsStringAsync();
                JObject shows = JObject.Parse(content);

                // Query json with linq
                // hardcode the query string per the requirement;alternatively this 
                // query string can be built from url parameters. 
                var jsonQuery = from show in shows["payload"]
                                where show["drm"] != null && (bool)show["drm"] == true
                                    && show["episodeCount"] != null && (int)show["episodeCount"] > 0
                                    && show["image"]["showImage"].Type != JTokenType.Undefined
                                    && show["slug"].Type != JTokenType.Undefined
                                    && show["title"].Type != JTokenType.Undefined
                                select new
                                {
                                    image = show["image"]["showImage"],
                                    slug = show["slug"],
                                    title = show["title"]
                                };

                // Serializing json with linq
                JArray jsonArray = new JArray(
                                             jsonQuery.Select(p => new JObject
                                             {
                                                 { "image", p.image },
                                                 { "slug", p.slug   },
                                                 { "title", p.title },
                                             })
                                            );

                // Add response key
                JObject jsonResponse = new JObject();
                jsonResponse["response"] = jsonArray;
                return Request.CreateResponse<JObject>(HttpStatusCode.OK, jsonResponse);
            }
            catch (Exception ex)  // Return error mesage if received an invalid JSON
            {
                JObject jsonError = new JObject();
                jsonError["error"] = "Could not decode request: JSON parsing failed";
                return Request.CreateResponse<JObject>(HttpStatusCode.BadRequest, jsonError);
            }
        }


        /// <summary>
        /// Determines whether the specified content is JSON indicated by a 
        /// content type of either <c>application/json</c>, <c>text/json</c>, <c>application/xyz+json</c>,
        /// or <c>text/xyz+json</c>. The term <c>xyz</c> can for example be <c>hal</c> or some other 
        /// JSON derived media type.
        /// </summary>
        /// <returns>true if the specified content is JSON content; otherwise, false.</returns>
        /// <param name="content">The content to check.</param>
        private bool IsJsonContent(HttpContent content)
        {
            if (content == null || content.Headers == null || content.Headers.ContentType == null || content.Headers.ContentType.MediaType == null)
            {
                return false;
            }

            string mediaType = content.Headers.ContentType.MediaType;
            return string.Equals(mediaType, "application/json", StringComparison.OrdinalIgnoreCase)
                || string.Equals(mediaType, "text/json", StringComparison.OrdinalIgnoreCase)
                || ((mediaType.StartsWith("application/", StringComparison.OrdinalIgnoreCase) || mediaType.StartsWith("text/", StringComparison.OrdinalIgnoreCase))
                    && mediaType.EndsWith("+json", StringComparison.OrdinalIgnoreCase));
        }

    }
}