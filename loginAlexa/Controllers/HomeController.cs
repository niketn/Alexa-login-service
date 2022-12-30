using loginAlexa.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace loginAlexa.Controllers
{
    public class HomeController : Controller
    {
        private IConfiguration configuration;
        public HomeController(IConfiguration iConfig)
        {
            configuration = iConfig;
        }
        [HttpPost]
        [Route("LoginService")]
        public async Task<string> Index(Request model)
        {
            using var client2 = new HttpClient();
            StreamReader r = new StreamReader("LoginRequest.json");
            string jsonString = r.ReadToEnd();

            //updating values based on post request of the API
            dynamic jsonObject = JsonConvert.DeserializeObject(jsonString);
            jsonObject.LoginId = model.UserName;
            jsonObject.Password = model.Password;
            jsonObject.EchoToken = Guid.NewGuid().ToString(); 
            jsonObject.LoginTime = DateTime.UtcNow.ToString();
            string modifiedJsonString = JsonConvert.SerializeObject(jsonObject);

            var content = new StringContent(modifiedJsonString, Encoding.UTF8, "application/json");
            
            var url2 = configuration.GetValue<string>("LoginMicroserviceUrl");// will read the URL for token request call
            var response2 = await client2.PostAsync(url2, content);
            string result2 = response2.Content.ReadAsStringAsync().Result;
            return result2;
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
