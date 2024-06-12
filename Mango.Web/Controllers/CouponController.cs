using Mango.Web.Models;
using Mango.Web.Models.DTO;
using Mango.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Mango.Web.Controllers
{
	public class CouponController : Controller
	{
		private readonly ICouponService couponService;

		public CouponController(ICouponService couponService)
        {
			this.couponService = couponService;
		}
        public async Task<IActionResult> CouponIndex()
		{

			List<CouponDto>? list = new();
			ResponseDto? response = await couponService.GetAllCouponseAsync();

			if(response != null && response.IsSuccess)
			{
				list = JsonSerializer.Deserialize<List<CouponDto>>(Convert.ToString(response.Result));
			}

			return View(list);
		}
	}
}
