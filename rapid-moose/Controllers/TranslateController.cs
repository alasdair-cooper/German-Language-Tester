using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

using System.Web;
using Microsoft.AspNetCore.Http;

namespace rapid_moose.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TranslateController : ControllerBase
    {
        private const string key_var = "TRANSLATOR_TEXT_SUBSCRIPTION_KEY";
        private static readonly string subscriptionKey = Environment.GetEnvironmentVariable(key_var);

        private const string endpoint_var = "TRANSLATOR_TEXT_ENDPOINT";
        private static readonly string endpoint = Environment.GetEnvironmentVariable(endpoint_var);

        private const string apikey_var = "API_KEY";
        private static readonly string apiKey = Environment.GetEnvironmentVariable(apikey_var);

        private readonly ILogger<TranslateController> _logger;

        private readonly string[] languages = new string[] { "en", "de" };

        public TranslateController(ILogger<TranslateController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<Dictionary<string, string>>> Get([FromHeader] int ApiAccessKey, [FromQuery] string text = null, [FromQuery] string to = "de", [FromQuery] string from = "en")
        {
            if (null == subscriptionKey)
            {
                throw new Exception("Please set/export the environment variable: " + key_var);
            }
            if (null == endpoint)
            {
                throw new Exception("Please set/export the environment variable: " + endpoint_var);
            }
            if (!Session.CheckSession(ApiAccessKey))
            {
                return StatusCode(StatusCodes.Status403Forbidden);
            }

            text = HttpUtility.HtmlDecode(text.Replace("_", ""));

            string route = $"/translate?api-version=3.0&to={to}&from={from}";
            text = await TranslateTextRequest(subscriptionKey, endpoint, route, text);
            Dictionary<string, string> output = new Dictionary<string, string>();
            output.Add(to, text);
            return Ok(output);
        }

        public async Task<string> TranslateTextRequest(string subscriptionKey, string endpoint, string route, string inputText)
        {
            object[] body = new object[] { new { Text = inputText } };
            var requestBody = JsonConvert.SerializeObject(body);

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(endpoint + route);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

                HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
                string result = await response.Content.ReadAsStringAsync();
                // Deserialize the response
                TranslationResult[] deserializedOutput = JsonConvert.DeserializeObject<TranslationResult[]>(result);

                string translatedText = "";

                foreach (TranslationResult o in deserializedOutput)
                {
                    foreach (TranslationResult.Translation t in o.Translations)
                    {
                        translatedText = translatedText + t.Text;
                    }
                }
                return (translatedText);
            }
        }
    }
}