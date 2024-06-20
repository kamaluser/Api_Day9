using CourseApp.UI.Filters;
using CourseApp.UI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace CourseApp.UI.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _client;

        public AccountController(HttpClient client)
        {
            _client = client;
        }

        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest loginRequest, string returnUrl = null)
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var content = new StringContent(JsonSerializer.Serialize(loginRequest, options), Encoding.UTF8, "application/json");

            using (var response = await _client.PostAsync("https://localhost:44392/api/Auth/login", content))
            {
                if (response.IsSuccessStatusCode)
                {
                    var loginResponse = JsonSerializer.Deserialize<LoginResponse>(await response.Content.ReadAsStringAsync(), options);
                    Response.Cookies.Append("token", loginResponse.Token);

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("index", "group");
                    }
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    ModelState.AddModelError("", "Email or Password is incorrect!");
                    ViewData["ReturnUrl"] = returnUrl;
                    return View();
                }
                else
                {
                    TempData["Error"] = "Something went wrong!";
                }
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }
    }
}
