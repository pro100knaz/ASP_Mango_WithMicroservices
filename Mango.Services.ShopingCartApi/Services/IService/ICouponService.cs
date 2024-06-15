using Mango.Services.ShopingCartApi.Models.DTO;

namespace Mango.Services.ShopingCartApi.Services.IService
{
    public interface ICouponService
	{
        Task<CouponDto> GetCoupon(string couponCode);
    }
}
