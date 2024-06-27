
using Mango.Services.ShopingCartApi.Models;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ShopingCartApi.Data
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
		{

		}
		public DbSet<CartHeader> CartHeaders { get; set; }
		public DbSet<CartDetails> CartDetails { get; set; }
	}
}
