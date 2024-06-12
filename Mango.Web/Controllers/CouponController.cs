﻿using Mango.Web.Models;
using Mango.Web.Models.DTO;
using Mango.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
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
			ResponseDto<List<CouponDto>>? response = await couponService.GetAllCouponseAsync();

			if(response != null && response.IsSuccess)
			{
				//list = // JsonSerializer.Deserialize<List<CouponDto>>(Convert.ToString(response.Result));
				list = JsonConvert.DeserializeObject<List<CouponDto>>(Convert.ToString(response.Result));
			}

			return View(list);
		}


		[HttpGet]
		public async Task<IActionResult> CouponCreate()
		{

			return View();
		}


		[HttpPost]
        public async Task<IActionResult> CouponCreate(CouponDto couponDto)
        {

			if(ModelState.IsValid)
			{
                
                ResponseDto? response = await couponService.CreateCouponsAsync(couponDto);

                if (response != null && response.IsSuccess)
                {
                    return RedirectToAction(nameof(CouponIndex));
                }
            }

			return View(couponDto);
			
        }


        [HttpGet]
        public async Task<IActionResult> CouponDelete(int couponId)
        {
            ResponseDto? response = await couponService.GetCouponeByIdAsync(couponId);

			if (response != null && response.IsSuccess)
			{
				CouponDto? model = JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(response.Result));

				return View(model);
			}


			return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> CouponDelete(CouponDto couponDto)
        {
            ResponseDto? response = await couponService.DeleteCouponseAsync(couponDto.CouponId);

            if (response != null && response.IsSuccess)
            {
                CouponDto? model = JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(response.Result));

                return RedirectToAction(nameof(CouponIndex));
            }


			return View(couponDto);
        }

    }
}
