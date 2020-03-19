// Copyright(c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using DirectLine.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace DirectLine.Web.Controllers
{
    public class HomeController : Controller
    {
        private string directLineSecret;
        private string botName;

        public const string GenerateDirectLineTokenUrl = "https://directline.botframework.com/v3/directline/tokens/generate";
        public const string AadObjectidentifierClaim = "http://schemas.microsoft.com/identity/claims/objectidentifier";

        public HomeController(IConfiguration configuration)
        {
            // Retrieve the Bot configuration
            directLineSecret = configuration.GetSection("DirectLineSecret").Value;
            botName = configuration.GetSection("BotName").Value;
        }

        public IActionResult Index(string locale = "en-us")
        {
            // Get DirectLine Token
            // Pass the DirectLine Token, Speech Key and Voice Name
            // Note this approach will require magic code validation
            var directLineToken = string.Empty;
            var directLineClient = new HttpClient();
            directLineClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", directLineSecret);

            var response = directLineClient.PostAsync(GenerateDirectLineTokenUrl, new StringContent(string.Empty, Encoding.UTF8, "application/json")).Result;

            if (response.IsSuccessStatusCode)
            {
                var responseString = response.Content.ReadAsStringAsync().Result;
                var directLineResponse = JsonConvert.DeserializeObject<DirectLineResponse>(responseString);
                directLineToken = directLineResponse.token;
            }

            ViewData["Title"] = botName;

            ViewData["DirectLineToken"] = directLineToken;

            ViewData["Locale"] = locale;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
