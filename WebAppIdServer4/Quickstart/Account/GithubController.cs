using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAppIdServer4.Quickstart.Account
{
    [Route("api/[controller]")]
    [ApiController]
    public class GithubController : ControllerBase
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public GithubController(IHttpContextAccessor _httpContextAccessor)
        {
            httpContextAccessor = _httpContextAccessor;
        }

        [HttpPost]
        public IActionResult GetGithubCode()
        {
            var request = httpContextAccessor.HttpContext.Request;

            return Ok("");
        }
    }
}
