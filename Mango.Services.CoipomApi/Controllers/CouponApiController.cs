using AutoMapper;
using Mango.Services.CouponApi.Data;
using Mango.Services.CouponApi.Models;
using Mango.Services.CouponApi.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.CouponApi.Controllers
{
	[Route("api/coupon")]
	[ApiController]
	public class CouponApiController : ControllerBase //special api controller
	{
		private readonly IMapper mapper;
		private readonly AppDbContext appDbContext;
		//private ResponseDto ResponseDto;
		public CouponApiController(IMapper mapper ,AppDbContext appDbContext)
        {
			this.mapper = mapper;
			this.appDbContext = appDbContext;
			//ResponseDto = new ResponseDto();
		}

		
		[HttpGet]
		public ResponseDto<List<CouponDto>> Get() {
			 ResponseDto<List<CouponDto>> ResponseDto  = new ResponseDto<List<CouponDto>>();
			try
			{
				IEnumerable<Coupon> objList = appDbContext.Coupons.ToList();
				ResponseDto.Result
					= mapper.Map<List<CouponDto>>(objList);
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
		public ResponseDto<CouponDto> Get(int id)
		{
			ResponseDto<CouponDto> ResponseDto = new ();
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
		public ResponseDto<CouponDto> GetByCode(string code)
		{
			ResponseDto<CouponDto> ResponseDto = new();
			try
			{
				Coupon? coupon = appDbContext.Coupons.FirstOrDefault(c => c.CouponCode.ToLower() == code.ToLower());
				if(code == null)
					ResponseDto.IsSuccess = false;
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
		public ResponseDto<CouponDto> Post([FromBody] CouponDto couponDto)
		{
			ResponseDto<CouponDto> ResponseDto = new();
			try
			{
				Coupon? coupon = mapper.Map<Coupon>(couponDto);
				if (coupon == null)
					ResponseDto.IsSuccess = false;
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
		public ResponseDto<CouponDto> Put([FromBody] CouponDto couponDto)
		{
			ResponseDto<CouponDto> ResponseDto = new();
			try
			{
				Coupon? coupon = mapper.Map<Coupon>(couponDto);
				if (coupon == null)
					ResponseDto.IsSuccess = false;
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
		public ResponseDto<CouponDto> Delete(int id)
		{
			ResponseDto<CouponDto> ResponseDto = new();
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
