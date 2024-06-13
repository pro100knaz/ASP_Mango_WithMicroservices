using Mango.Web.Models;
using Mango.Web.Models.DTO;
using Mango.Web.Services.IService;
using Mango.Web.Utility;

namespace Mango.Web.Services
{
    public class AuthService : IAuthService
    {
        private readonly IBaseService baseService;
        public AuthService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<ResponseDto?> AssignRoleAsync(RegistrationRequestDto model)
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = model,
                Url = SD.AuthApiBase + "/api/auth/AssignRole"
            }, withBeare:false);
        }

        public async Task<ResponseDto?> LoginAsync(LoginRequestDto model)
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = model,
                Url = SD.AuthApiBase + "/api/auth/login"
            }, withBeare: false);
        }

        public async Task<ResponseDto?> RegisterAsync(RegistrationRequestDto model)
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = model,
                Url = SD.AuthApiBase + "/api/auth/register"
            }, withBeare: false);
        }
    }
}
 