using Mango.Services.RewardApi.MEssages;
using Microsoft.EntityFrameworkCore;
using Mango.Services.RewardApi.Data;
using Mango.Services.RewardApi.Models;


namespace Mango.Services.RewardApi.Services
{
    public class RewardService : IRewardService
    {
        private readonly DbContextOptions<AppDbContext> _dbOptions;

        public RewardService(DbContextOptions<Data.AppDbContext> options)
        {
            _dbOptions = options;
        }
        public async Task<bool> UpdateRewards(RewardsMessages rewardsMessages)
        {
            try
            {
                Reward reward = new()
                {
                    RewardActivity = rewardsMessages.RewardActivity,
                    OrderId = rewardsMessages.OrderId,
                    UserId = rewardsMessages.UserId,
                    RewardDate = DateTime.Now,
                };
                await using var _db = new AppDbContext(_dbOptions);

                _db.Rewards.Add(reward);

                await _db.SaveChangesAsync();


                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }
    }
}
