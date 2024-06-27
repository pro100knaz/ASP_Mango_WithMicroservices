using System.ComponentModel.DataAnnotations;

namespace Mango.Services.RewardApi.Models
{
	public class Reward
	{
		[Key]
		public int Id { get; set; }
		public string UserId { get; set; }

		public DateTime RewardDate { get; set; }

		public int RewardActivity { get; set; }
		public int OrderId { get; set; }

	}
}
