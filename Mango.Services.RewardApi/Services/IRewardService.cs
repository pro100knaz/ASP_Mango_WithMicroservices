using Mango.Services.RewardApi.MEssages;

namespace Mango.Services.RewardApi.Services
{
    public interface IRewardService
    {
        Task<bool> UpdateRewards(RewardsMessages rewardsMessages);
    }
}
