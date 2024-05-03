using Mango.Services.EmailAPI.Data;
using Mango.Services.EmailAPI.Message;
using Mango.Services.EmailAPI.Models;
using Mango.Services.EmailAPI.Models.Dto;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Mango.Services.EmailAPI.Service
{
    public class EmailService : IEmailService
    {
        private DbContextOptions<AppDBContext> _dbOptions;

        public EmailService(DbContextOptions<AppDBContext> dbOptions)
        {
            _dbOptions = dbOptions;
        }

        public async Task EmailCartAndLog(CartDto dto)
        {
           StringBuilder message=new StringBuilder();
            message.AppendLine("<br/>Cart Email Requested ");
            message.AppendLine("<br/>Total " + dto.CartHeader.CartTotal);
            message.Append("<br/>");
            message.Append("<ul>");

            foreach (var item in dto.CartDetails)
            {
                message.Append("<li>");
                message.Append(item.Product.Name + " x " + item.Count);
                message.Append("</li>");
            }
            message.Append("</ul>");
                
            await LogAndEmail(message.ToString(), dto.CartHeader.Email);
        }

        public async Task LogOrderPlaced(RewardsMessage rewardsMessage)
        {
            string message = "New order placed.</br>Order Id: " + rewardsMessage.OrderId;
            await LogAndEmail(message.ToString(), "amitupadhyay5912@gmail.com");
        }

        public async Task RegisterUserEmailAndLog(string email)
        {
            await LogAndEmail(email, "amitupadhyay5912@gmail.com");
        }

        private async Task<bool> LogAndEmail(string message,string email)
        {
            try
            {
                EmailLogger logger = new EmailLogger()
                {
                    Email = email,
                    EmailSentOn = DateTime.Now,
                    Message = message
                };

                await using var _db = new AppDBContext(_dbOptions);
                await _db.EmailLoggers.AddAsync(logger);
                await _db.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
