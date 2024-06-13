using Mango.Services.AuthAPI.Models.DTO;

namespace Mango.Services.AuthAPI.Services.IService
{
    public interface IAuthService
    {
        Task<string> Register(RegistrationRequestDto registrationRequestDto);
        Task<LoginResponseDto> Login(LoginRequestDto registrationRequestDto);

        Task<bool> AssignRole(string Email, string roleName);
    }
}
