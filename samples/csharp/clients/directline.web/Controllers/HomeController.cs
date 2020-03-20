// Copyright(c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Diagnostics;
using System.Net.Http;
using System.Text;
using DirectLine.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DirectLine.Web.Controllers
{
    public class HomeController : Controller
    {
        private const string SpeechServiceTokenUriFormat = "https://{0}.api.cognitive.microsoft.com/sts/v1.0/issuetoken";
        private const string SubscriptionKeyHeaderName = "Ocp-Apim-Subscription-Key";

        private string botName;
        private string speechServiceRegionIdentifier;
        private string speechServiceSubscriptionKey;
        private string speechServiceTokenUri;

        public HomeController(IConfiguration configuration)
        {
            // Retrieve the Bot configuration
            this.botName = configuration.GetSection("BotName").Value;
            this.speechServiceRegionIdentifier = configuration.GetSection("SpeechServiceRegionIdentfier").Value;
            this.speechServiceSubscriptionKey = configuration.GetSection("SpeechServiceSubscriptionKey").Value;
            this.speechServiceTokenUri = string.Format(SpeechServiceTokenUriFormat, this.speechServiceRegionIdentifier);
        }

        public IActionResult Index(string locale = "en-us")
        {
            // Get DirectLine Token
            // Pass the DirectLine Token, Speech Key and Voice Name
            // Note this approach will require magic code validation
            string speechServiceAuthorizationToken = string.Empty;
            var directLineClient = new HttpClient();
            directLineClient.DefaultRequestHeaders.Add(SubscriptionKeyHeaderName, this.speechServiceSubscriptionKey);

            HttpResponseMessage response = directLineClient.PostAsync(
                this.speechServiceTokenUri,
                new StringContent(string.Empty, Encoding.UTF8, "application/x-www-form-urlencoded")).Result;

            if (response.IsSuccessStatusCode)
            {
                speechServiceAuthorizationToken = response.Content.ReadAsStringAsync().Result;
            }

            ViewData["Locale"] = locale;
            ViewData["SpeechServiceAuthorizationToken"] = speechServiceAuthorizationToken;
            ViewData["SpeechServiceRegionIdentifier"] = this.speechServiceRegionIdentifier;
            ViewData["Title"] = botName;

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
