using Mango.Web.Services.IService;
using Mango.Web.Utility;
using Newtonsoft.Json.Linq;

namespace Mango.Web.Services
{
    public class TokenProvider : ITokenProvider // it is necessary t o use cookies
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public TokenProvider(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }
        public void ClearToken()
        {
            httpContextAccessor.HttpContext.Response.Cookies.Delete(SD.TokenCookie);
        }

        public string? GetToken()
        {
            string? token = null;

            bool? hasToken = httpContextAccessor.HttpContext?.Request.Cookies.TryGetValue(SD.TokenCookie, out token);

            return hasToken is true ? token : null;
        }

        public void SetToken(string token)
        {
            httpContextAccessor.HttpContext.Response.Cookies.Append(SD.TokenCookie, token);

        }
    }
}
