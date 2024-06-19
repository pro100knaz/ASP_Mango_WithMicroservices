using Mango.MessageBus;
using Mango.Services.AuthAPI.Models.DTO;
using Mango.Services.AuthAPI.RabbitMqSender;
using Mango.Services.AuthAPI.Services.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Mango.Services.AuthAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthAPIController : ControllerBase
    {
        private readonly IAuthService authService;
        private readonly IRabbitMqAuthMessageSender messageBus;
        private readonly IConfiguration configuration;
        private ResponseDto responseDto;

        public AuthAPIController(IAuthService authService,
			IRabbitMqAuthMessageSender messageBus,
            IConfiguration configuration)
        {
            this.authService = authService;
            this.messageBus = messageBus;
            this.configuration = configuration;
            responseDto = new();
        }

		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] RegistrationRequestDto model)
		{

			var errorMessage = await authService.Register(model);
			if (!string.IsNullOrEmpty(errorMessage))
			{
				responseDto.IsSuccess = false;
				responseDto.Message = errorMessage;
                return BadRequest(responseDto);
            }


            var x = configuration.GetValue<string>("TopicAndQueueNames:RegisterUserQueue");


            messageBus.SendMessage(model.Email, configuration.GetValue<string>("TopicAndQueueNames:RegisterUserQueue"));



			return Ok(responseDto);
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
