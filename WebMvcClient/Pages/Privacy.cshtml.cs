using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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
            string accessToken = _contextAccessor.HttpContext.GetTokenAsync("access_token").Result;
            string idToken = _contextAccessor.HttpContext.GetTokenAsync("id_token").Result;
            var claimsList = from c in User.Claims select new { c.Type, c.Value };
        }
    }
}