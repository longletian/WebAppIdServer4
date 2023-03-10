using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Net.Http.Headers;
using System.Text.Json;

namespace WebMvcClient.Pages
{
    [Authorize]
    public class PrivacyModel : PageModel
    {
        private readonly ILogger<PrivacyModel> _logger;
        private readonly IHttpContextAccessor _contextAccessor;
        public PrivacyModel(ILogger<PrivacyModel> logger, IHttpContextAccessor contextAccessor)
        {
            _logger = logger;
            _contextAccessor = contextAccessor;
        }

        public void OnGet()
        {
            ViewData["accessToken"] = _contextAccessor.HttpContext.GetTokenAsync("access_token").Result;
            ViewData["idToken"] = _contextAccessor.HttpContext.GetTokenAsync("id_token").Result;
            ViewData["refreshToken"] = _contextAccessor.HttpContext.GetTokenAsync("refreshToken").Result;
        }

        public async Task<IActionResult> OnPost()
        {
            var client = new HttpClient();
            var accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            if (string.IsNullOrEmpty(accessToken))
            {
                return new  JsonResult(new { msg = "accesstoken 获取失败" });
            }
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var httpResponse = await client.GetAsync("http://localhost:5001/identity");
            var result = await httpResponse.Content.ReadAsStringAsync();
            if (!httpResponse.IsSuccessStatusCode)
            {
                return new JsonResult(new { msg = "请求 api1 失败。", error = result });
            }
            return new JsonResult(new
            {
                msg = "成功",
                data = JsonSerializer.Deserialize<object>(result)
            });
        }
    }
}