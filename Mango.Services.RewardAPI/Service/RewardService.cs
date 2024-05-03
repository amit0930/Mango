using Mango.Services.RewardAPI.Data;
using Mango.Services.RewardAPI.Message;
using Mango.Services.RewardAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Mango.Services.RewardAPI.Service
{
    public class RewardService : IRewardService
    {
        private DbContextOptions<AppDBContext> _dbOptions;

        public RewardService(DbContextOptions<AppDBContext> dbOptions)
        {
            _dbOptions = dbOptions;
        }

      

        public async Task UpdateRewards(RewardsMessage message)
        {
            try
            {
                Rewards rewards = new()
                {
                    UserId = message.UserId,
                    RewardsActivity = message.RewardsActivity,
                    OrderId = message.OrderId,
                    RewardsDate=DateTime.Now
                };

                await using var _db = new AppDBContext(_dbOptions);
                await _db.Rewards.AddAsync(rewards);
                await _db.SaveChangesAsync();

                
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
