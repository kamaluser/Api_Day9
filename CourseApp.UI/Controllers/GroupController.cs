using CourseApp.UI.Filters;
using CourseApp.UI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace CourseApp.UI.Controllers
{
    [ServiceFilter(typeof(AuthFilter))]
    public class GroupController : Controller
    {
        private readonly HttpClient _client;

        public GroupController(HttpClient client)
        {
            _client = client;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var token = Request.Cookies["token"];

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using (var response = await _client.GetAsync($"https://localhost:44392/api/Groups?page={page}&size=2"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var bodyStr = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var data = JsonSerializer.Deserialize<PaginatedResponse<GroupListItemGetResponse>>(bodyStr, options);

                    if (data.TotalPages < page)
                    {
                        return RedirectToAction("index", new { page = data.TotalPages });
                    }

                    return View(data);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("login", "account");
                }
                else
                {
                    return RedirectToAction("error", "home");
                }
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(GroupCreateRequest dto)
        {
            var token = Request.Cookies["token"];

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            if (!ModelState.IsValid)
            {
                return View();
            }

            var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
            using (var response = await _client.PostAsync("https://localhost:44392/api/Groups", content))
            {
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("index");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("login", "account");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseContent, options);

                    foreach (var item in errorResponse.Errors)
                    {
                        ModelState.AddModelError(item.Key, item.Message);
                    }

                    return View();
                }
                else
                {
                    TempData["Error"] = "Something went wrong!";
                }
            }

            return View(dto);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var token = Request.Cookies["token"];

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using (var response = await _client.GetAsync($"https://localhost:44392/api/Groups/{id}"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var request = JsonSerializer.Deserialize<GroupCreateRequest>(await response.Content.ReadAsStringAsync(), options);
                    return View(request);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("login", "account");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    TempData["Error"] = "Group Not Found!";
                }
                else
                {
                    TempData["Error"] = "Something went wrong!";
                }
            }

            return RedirectToAction("index");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(GroupCreateRequest editRequest, int id)
        {
            var token = Request.Cookies["token"];

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            if (!ModelState.IsValid) return View();

            var content = new StringContent(JsonSerializer.Serialize(editRequest), Encoding.UTF8, "application/json");
            using (var response = await _client.PutAsync($"https://localhost:44392/api/Groups/{id}", content))
            {
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("index");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("login", "account");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(await response.Content.ReadAsStringAsync(), options);

                    foreach (var item in errorResponse.Errors)
                    {
                        ModelState.AddModelError(item.Key, item.Message);
                    }

                    return View();
                }
                else
                {
                    TempData["Error"] = "Something went wrong!";
                }
            }

            return View(editRequest);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var token = Request.Cookies["token"];

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using (var response = await _client.DeleteAsync($"https://localhost:44392/api/Groups/{id}"))
            {
                if (response.IsSuccessStatusCode)
                {
                    return Ok();
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return Unauthorized();
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return NotFound();
                }
                else
                {
                    return StatusCode(500);
                }
            }
        }
    }
}