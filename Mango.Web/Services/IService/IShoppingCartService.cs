using Mango.Web.Models;
using Mango.Web.Models.DTO;

namespace Mango.Web.Services.IService
{
	public interface IShoppingCartService
	{
		Task<ResponseDto?> GetCartByUSerIdAsync(string userId);
		Task<ResponseDto?> UpsertCartAsync(CartDto cartDto);
		Task<ResponseDto?> RemoveFromCartAsync(int cartDetails);
		Task<ResponseDto?> ApplyCouponAsync(CartDto cartDto);

	}
}
