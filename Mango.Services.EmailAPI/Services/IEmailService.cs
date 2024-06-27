
using Mango.Services.EmailAPI.MEssages;
using Mango.Services.EmailAPI.Models.DTO;

namespace Mango.Services.EmailAPI.Services
{
	public interface IEmailService
	{
		Task EmailCartAndLog(CartDto cartDto);

		Task RegisterUserEmailLog(string email);

		Task LogOrderPlaced(RewardsMessages rewardsMessages);
	}
}
