using Mango.Services.EmailAPI.Data;
using Mango.Services.EmailAPI.Models;
using Mango.Services.EmailAPI.Models.DTO;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Mango.Services.EmailAPI.Services
{
    public class EmailService : IEmailService
    {
        private readonly DbContextOptions<AppDbContext> _dbOptions;

        public EmailService(DbContextOptions<Data.AppDbContext> options)
        {
            _dbOptions = options;
        }
        public async Task EmailCartAndLog(CartDto cartDto)
        {
            StringBuilder message = new StringBuilder();

                message.AppendLine("<br/> Cart Email Requested");
                message.AppendLine("<br/> Total" + cartDto.CartHeader.CartTotal);
                message.AppendLine("<br/>");
                message.Append("<ul>");
                foreach (var item in cartDto.CartDetails)
                {
                    message.Append("<li>");
                    message.Append(item.Product.Name + "  x  " + item.Count);
                    message.Append("</li>");
                }
                message.Append("</ul>");

             await LogAndEmail(message.ToString(), cartDto.CartHeader.Email);
        }

        public async Task RegisterUserEmailLog(string email)
        {
            StringBuilder message = new StringBuilder();

            message.AppendLine("New USer registrated succesfully");

            await LogAndEmail(message.ToString(), email);
        }

        private async Task<bool> LogAndEmail(string message, string email)
        {
            try
            {
                EmailLogger emailLogger = new()
                {

                    Email = email,
                    EmailSent = DateTime.Now,
                    Message = message
                };
                await using var _db = new AppDbContext(_dbOptions);

                _db.emailLoggers.Add(emailLogger);

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
