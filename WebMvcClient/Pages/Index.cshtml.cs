using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebMvcClient.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IHttpContextAccessor _contextAccessor;
        public IndexModel(ILogger<IndexModel> logger, IHttpContextAccessor contextAccessor)
        {
            _logger = logger;
            _contextAccessor = contextAccessor;
        }
   
        public void OnGet()
        {
            var request = _contextAccessor.HttpContext.Request;
            var url = $"{request.Scheme}://{request.Host}{request.PathBase}{request.Path}-callback?provider=GitHub&redirectUrl=";

            HttpRequest httpRequest = _contextAccessor.HttpContext.Request;

 
        }
    }
}