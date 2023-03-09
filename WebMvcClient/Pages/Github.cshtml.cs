using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebMvcClient.Services;

namespace WebMvcClient.Pages
{
    public class GithubModel : PageModel
    {
        private readonly ILogger<PrivacyModel> _logger;
        private readonly IHttpContextAccessor _contextAccessor;
        public GithubModel(ILogger<PrivacyModel> logger, IHttpContextAccessor contextAccessor)
        {
            _logger = logger;
            _contextAccessor = contextAccessor;
        }

        public void OnGet()
        {
            ViewData["accessToken"] = _contextAccessor.HttpContext.GetTokenAsync("access_token").Result;
            ViewData["idToken"] = _contextAccessor.HttpContext.GetTokenAsync("id_token").Result;
        }

    }
}
