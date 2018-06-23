using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Bootstrap.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using System.Text;

namespace Bootstrap.Controllers
{
    public class HomeController : Controller
    {
        string newToken;

        public IActionResult Register()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string userName, string password, string returnUrl = null)
        {
            if (string.IsNullOrEmpty(userName)) return BadRequest("A user name is required");
            if (string.IsNullOrEmpty(password)) return BadRequest("A password is required");

            // Call login api (authenticate using username and password, return token if successful)
            await GetToken(userName, password);


            // If the user login is successful
            string userId = "";
            string username = "";

            if (newToken != null)
            {
                // Call api GetUserInfo by passing the user token as header
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", newToken);

                HttpResponseMessage response = await client.GetAsync("http://localhost:5000/api/userinfo");
                response.EnsureSuccessStatusCode();
                var responseData = response.Content.ReadAsStringAsync();
                var userData = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseData.Result.ToString());

                foreach (var userInfo in userData)
                {
                    if (userInfo.Key == "userId")
                        userId = userInfo.Value;

                    if (userInfo.Key == "username")
                        username = userInfo.Value;
                }

                client.Dispose();
            }


            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.NameIdentifier, userId));
            claims.Add(new Claim(ClaimTypes.Name, username));
            claims.Add(new Claim(ClaimTypes.Authentication, newToken));

            // Create user's identity and sign them in to create the Http Context
            var identity = new ClaimsIdentity(claims, "UserSpecified");
            await HttpContext.SignInAsync(new ClaimsPrincipal(identity));  

            return Redirect("/Home/Dashboard");
        }

        public async Task<string> GetToken(string userName, string password)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var jsonData = new StringContent(JsonConvert.SerializeObject(new { MobileNo = userName, Password = password }), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("http://localhost:5000/api/login", jsonData);
            response.EnsureSuccessStatusCode();

            var responseData1 = response.Content.ReadAsStringAsync();
            newToken = JsonConvert.DeserializeObject(responseData1.Result).ToString();
            client.Dispose();

            return newToken;
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        public IActionResult Index()
        {
            return View("~/Views/Home/Login.cshtml");
        }        

        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult Cards()
        {
            return View();
        }

        public IActionResult Tables()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/Home/Login");
        }
    }
}
