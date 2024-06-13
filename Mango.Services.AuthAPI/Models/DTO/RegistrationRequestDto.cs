using System.ComponentModel.DataAnnotations;

namespace Mango.Services.AuthAPI.Models.DTO
{
    public class RegistrationRequestDto
    {
        public string Name { get; set; }
        [EmailAddress(ErrorMessage ="Invalid Email")]
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string? Role { get; set; }
    }
}
