using CourseApp.UI.Filters;
using CourseApp.UI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace CourseApp.UI.Controllers
{
    [ServiceFilter(typeof(AuthFilter))]
    public class StudentController : Controller
    {
        private readonly HttpClient _client;

        public StudentController(HttpClient client)
        {
            _client = client;
        }

        private async Task<List<GroupListItemGetResponse>> GetGroupsAsync()
        {
            var token = Request.Cookies["token"];
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync("https://localhost:44392/api/groups");
            response.EnsureSuccessStatusCode();

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var data = JsonSerializer.Deserialize<PaginatedResponse<GroupListItemGetResponse>>(await response.Content.ReadAsStringAsync(), options);
            return data.Items;
        }

        public async Task<IActionResult> Index(int page = 1, int size = 4)
        {
            var token = Request.Cookies["token"];
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var queryString = new StringBuilder();
            queryString.Append("?page=").Append(Uri.EscapeDataString(page.ToString()));
            queryString.Append("&size=").Append(Uri.EscapeDataString(size.ToString()));

            string requestUrl = "https://localhost:44392/api/students" + queryString;
            var response = await _client.GetAsync(requestUrl);

            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var data = JsonSerializer.Deserialize<PaginatedResponse<StudentListItemGetResponse>>(await response.Content.ReadAsStringAsync(), options);
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

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var groups = await GetGroupsAsync();
            ViewBag.Groups = groups;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(StudentCreateRequest dto)
        {
            var token = Request.Cookies["token"];
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            if (!ModelState.IsValid)
            {
                ViewBag.Groups = await GetGroupsAsync();
                return View(dto);
            }

            var multipartContent = new MultipartFormDataContent();
            multipartContent.Add(new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json"), "json");

            if (dto.Photo != null && dto.Photo.Length > 0)
            {
                multipartContent.Add(new StreamContent(dto.Photo.OpenReadStream()), "file", dto.Photo.FileName);
            }

            var response = await _client.PostAsync("https://localhost:44392/api/Students", multipartContent);
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

                TempData["Error"] = "Error details: " + string.Join(", ", errorResponse.Errors.Select(e => e.Message));
                ViewBag.Groups = await GetGroupsAsync();
                return View(dto);
            }
            else
            {
                TempData["Error"] = "Something went wrong!";
            }

            ViewBag.Groups = await GetGroupsAsync();
            return View(dto);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var token = Request.Cookies["token"];
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync($"https://localhost:44392/api/Students/{id}");
            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var student = JsonSerializer.Deserialize<StudentCreateRequest>(await response.Content.ReadAsStringAsync(), options);
                ViewBag.Groups = await GetGroupsAsync();
                return View(student);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("login", "account");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                TempData["Error"] = "Student not found";
            }
            else
            {
                TempData["Error"] = "Something went wrong!";
            }

            return RedirectToAction("index");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(StudentCreateRequest editRequest, int id)
        {
            var token = Request.Cookies["token"];
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            if (!ModelState.IsValid)
            {
                ViewBag.Groups = await GetGroupsAsync();
                return View(editRequest);
            }

            var multipartContent = new MultipartFormDataContent();
            multipartContent.Add(new StringContent(JsonSerializer.Serialize(editRequest), Encoding.UTF8, "application/json"), "json");

            if (editRequest.Photo != null && editRequest.Photo.Length > 0)
            {
                multipartContent.Add(new StreamContent(editRequest.Photo.OpenReadStream()), "file", editRequest.Photo.FileName);
            }

            var response = await _client.PutAsync($"https://localhost:44392/api/Students/{id}", multipartContent);
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

                TempData["Error"] = "Error details: " + string.Join(", ", errorResponse.Errors.Select(e => e.Message));
                ViewBag.Groups = await GetGroupsAsync();
                return View(editRequest);
            }
            else
            {
                TempData["Error"] = "Something went wrong!";
            }

            ViewBag.Groups = await GetGroupsAsync();
            return View(editRequest);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var token = Request.Cookies["token"];
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.DeleteAsync($"https://localhost:44392/api/Students/{id}");
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
