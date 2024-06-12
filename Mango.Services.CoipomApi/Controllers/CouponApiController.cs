using AutoMapper;
using Mango.Services.CouponApi.Data;
using Mango.Services.CouponApi.Models;
using Mango.Services.CouponApi.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.CouponApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CouponApiController : ControllerBase //special api controller
	{
		private readonly IMapper mapper;
		private readonly AppDbContext appDbContext;
		private ResponseDto ResponseDto;
		public CouponApiController(IMapper mapper ,AppDbContext appDbContext)
        {
			this.mapper = mapper;
			this.appDbContext = appDbContext;
			ResponseDto = new ResponseDto();
		}

		
		[HttpGet]
		public ResponseDto Get() {

			try
			{
				IEnumerable<Coupon> objList = appDbContext.Coupons.ToList();
				ResponseDto.Result = mapper.Map<IEnumerable<CouponDto>>(objList);
			}
			catch (Exception ex)
			{
				ResponseDto.Message = ex.Message;
				ResponseDto.IsSuccess = false; 
				//throw;
			}

			return ResponseDto;
			
		}

		[HttpGet]
		[Route("{id:int}")]
		public ResponseDto Get(int id)
		{
			try
			{		
				Coupon coupon = appDbContext.Coupons.First(c => c.CouponId == id); //can use FirstOrDefault cause it doesn't trow an exception
				
				ResponseDto.Result = mapper.Map<CouponDto>(coupon);
			}
			catch (Exception ex)
			{
				ResponseDto.Message = ex.Message;
				ResponseDto.IsSuccess = false;
				//throw;
			}

			return ResponseDto;

		}



		[HttpGet]
		[Route("GetByCode/{code}")]
		public ResponseDto GetByCode(string code)
		{
			try
			{
				Coupon? coupon = appDbContext.Coupons.FirstOrDefault(c => c.CouponCode.ToLower() == code.ToLower());
				if(code == null)
					ResponseDto.Result = false;
				ResponseDto.Result = mapper.Map<CouponDto>(coupon);
			}
			catch (Exception ex)
			{
				ResponseDto.Message = ex.Message;
				ResponseDto.IsSuccess = false;
				//throw;
			}

			return ResponseDto;

		}

		[HttpPost]
		public ResponseDto Post([FromBody] CouponDto couponDto)
		{
			try
			{
				Coupon? coupon = mapper.Map<Coupon>(couponDto);
				if (coupon == null)
					ResponseDto.Result = false;
				appDbContext.Coupons.Add(coupon);
				appDbContext.SaveChanges();

				ResponseDto.Result = mapper.Map<CouponDto>(coupon);
			}
			catch (Exception ex)
			{
				ResponseDto.Message = ex.Message;
				ResponseDto.IsSuccess = false;
				//throw;-
			}

			return ResponseDto;

		}

		[HttpPut]
		public ResponseDto Put([FromBody] CouponDto couponDto)
		{
			try
			{
				Coupon? coupon = mapper.Map<Coupon>(couponDto);
				if (coupon == null)
					ResponseDto.Result = false;
				appDbContext.Coupons.Update(coupon);
				appDbContext.SaveChanges();

				ResponseDto.Result = mapper.Map<CouponDto>(coupon);
			}
			catch (Exception ex)
			{
				ResponseDto.Message = ex.Message;
				ResponseDto.IsSuccess = false;
				//throw;-
			}

			return ResponseDto;

		}

		[HttpDelete]
		[Route("{id:int}")]
		public ResponseDto Delete(int id)
		{
			try
			{
				Coupon? coupon = appDbContext.Coupons.FirstOrDefault(c => c.CouponId == id); //can use FirstOrDefault cause it doesn't trow an exception
				var x = appDbContext.Coupons.Remove(coupon);
				appDbContext.SaveChanges();
			}
			catch (Exception ex)
			{
				ResponseDto.Message = ex.Message;
				ResponseDto.IsSuccess = false;
				//throw;-
			}

			return ResponseDto;

		}

	}
}
