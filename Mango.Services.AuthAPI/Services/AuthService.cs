using Mango.Services.AuthAPI.Data;
using Mango.Services.AuthAPI.Models;
using Mango.Services.AuthAPI.Models.DTO;
using Mango.Services.AuthAPI.Services.IService;
using Microsoft.AspNetCore.Identity;

namespace Mango.Services.AuthAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext appDbContext;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IJwtTokenGenerator jwtTokenGenerator;

        public AuthService(AppDbContext appDbContext, 
            UserManager<ApplicationUser> userManager, 
            RoleManager<IdentityRole> roleManager,
            IJwtTokenGenerator jwtTokenGenerator)
        {
            this.appDbContext = appDbContext;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.jwtTokenGenerator = jwtTokenGenerator;
        }

        
        public async Task<bool> AssignRole(string Email, string roleName)
        {
            var user = appDbContext.ApplicationUsers.FirstOrDefault(u => u.UserName == Email);
            if(user != null)
            {
                if (!roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
                {
                    roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
                }

                await userManager.AddToRoleAsync(user, roleName);
                return true;
            }
            return false;
        }

        public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
        {
            var user = appDbContext.ApplicationUsers.FirstOrDefault(u => u.UserName == loginRequestDto.UserName);

            bool isValid = await userManager.CheckPasswordAsync(user, loginRequestDto.Password);
            if (user is null || isValid == false)
            {
                return new LoginResponseDto() { Token="", User = null};
            }

            //if found have to JWT Token
            var roles = await userManager.GetRolesAsync(user);
            var token =  jwtTokenGenerator.GenerateToken(user, roles
                );

            //


            UserDto userDto = new() 
            {
                Email = user.Email,
                ID = user.Id,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber
            };



            LoginResponseDto loginResponseDto = new LoginResponseDto()
            {
                User = userDto,
                Token = token,
            };

            return loginResponseDto;
        }

        public async Task<string> Register(RegistrationRequestDto registrationRequestDto)
        {
            ApplicationUser user = new ApplicationUser()
            {
                UserName = registrationRequestDto.Email,
                Name = registrationRequestDto.Name,
                Email = registrationRequestDto.Email,
                NormalizedEmail = registrationRequestDto.Email.ToUpper(),
                PhoneNumber = registrationRequestDto.PhoneNumber,

            };
            try
            {

                //if()

                var result = await userManager.CreateAsync(user, registrationRequestDto.Password); //all will be done auto

                if (result.Succeeded)
                {
                    var userToReturn = appDbContext.ApplicationUsers.First(u => u.UserName == registrationRequestDto.Email);

                    UserDto userDto = new UserDto()
                    {
                        Email = user.Email,
                        ID = user.Id,
                        Name = user.Name,
                        PhoneNumber = user.PhoneNumber,
                    };

                    //we have to send new requst ti server bus to log new registered user

                     //inside controller

                    //


                    return "";

                }
                else
                {
                    return result.Errors.FirstOrDefault().Description;
                }

            }
            catch (Exception ex)
            {

                return @$"error Encounted {ex.Message}";

            }

        }
    }
}
