using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Net.Http.Headers;
using System.Text.Json;

namespace WebMvcClient.Services
{
    public class RequestService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        public RequestService(IHttpContextAccessor _httpContextAccessor) 
        {
            this.httpContextAccessor = _httpContextAccessor;
        }

        public async Task<JsonResult> GetJsonResult()
        {
            var client = new HttpClient();
            var accessToken = await httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            if (string.IsNullOrEmpty(accessToken))
            {
                return new JsonResult(new { msg = "accesstoken 获取失败" });
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
                data = JsonSerializer.Serialize(result)
            });
        }
    }
}
