using Mango.Services.AuthAPI.Models.DTO;
using Mango.Services.AuthAPI.Services.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.AuthAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthAPIController : ControllerBase
    {
        private readonly IAuthService authService;
        private ResponseDto responseDto;

        public AuthAPIController(IAuthService authService)
        {
            this.authService = authService;
            responseDto = new();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]RegistrationRequestDto model)
        {
            var errorMessage = await authService.Register(model);
            if(string.IsNullOrEmpty(errorMessage))
            {
                 return Ok(responseDto);
            }
            else
            {
                responseDto.Message = errorMessage;
                responseDto.IsSuccess = false; 
                return BadRequest(responseDto);
            }

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]LoginRequestDto loginRequestDto)
        {

            var loginResponse = await authService.Login(loginRequestDto);
            if(loginResponse.User is null)
            {
                responseDto.IsSuccess = false;
                responseDto.Message = "Incorrect UserName or Password";
                return BadRequest(responseDto);
            }

            responseDto.IsSuccess = true;
            responseDto.Result = loginResponse;
            return Ok(responseDto);
        }


        [HttpPost("AssignRole")]
        public async Task<IActionResult> AssignRole([FromBody]RegistrationRequestDto model)
        {

            var result = await authService.AssignRole(model.Email, model.Role.ToUpper());
            if (!result)
            {
                responseDto.IsSuccess = false;
                responseDto.Message = "Error encountered";
                return BadRequest(responseDto);
            }

           
            return Ok(responseDto);
        }

    }
}
