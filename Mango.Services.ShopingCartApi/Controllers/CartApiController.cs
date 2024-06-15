using AutoMapper;
using Azure;
using Mango.Services.ShopingCartApi.Data;
using Mango.Services.ShopingCartApi.Models;
using Mango.Services.ShopingCartApi.Models.DTO;
using Mango.Services.ShopingCartApi.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.PortableExecutable;

namespace Mango.Services.ShopingCartApi.Controllers
{
	[Route("api/cart")]
	[ApiController]
	//[Authorize]
	public class CartApiController : ControllerBase
	{
		private readonly AppDbContext appDbContext;
		private readonly IMapper mapper;
		private readonly IProductService productService;
		private readonly ICouponService couponService;
		private ResponseDto _responseDto;

		public CartApiController(AppDbContext appDbContext, IMapper mapper, IProductService productService, ICouponService couponService)
		{
			this.appDbContext = appDbContext;
			this.mapper = mapper;
			this.productService = productService;
			this.couponService = couponService;
			_responseDto = new ResponseDto();
		}



		[HttpPost("ApplyCoupon")]
		public async Task<ResponseDto> ApplyCoupon([FromBody] CartDto cartDto)
		{
			try
			{
				var cartFromDb = await appDbContext.CartHeaders.FirstAsync(u => u.UserId == cartDto.CartHeader.UserId);
				cartFromDb.CouponCode = cartDto.CartHeader.CouponCode;
				appDbContext.CartHeaders.Update(cartFromDb);
				await appDbContext.SaveChangesAsync();
				_responseDto.Result = true;
			}
			catch (Exception ex)
			{
				_responseDto.IsSuccess = false;
				_responseDto.Message = ex.ToString();
			}
			return _responseDto;
		}



		//[HttpPost("RemoveCoupon")]
		//public async Task<ResponseDto> RemoveCoupon([FromBody] CartDto cartDto)
		//{
		//	try
		//	{
		//		var cartFromDb = await appDbContext.CartHeaders.FirstAsync(u => u.UserId == cartDto.CartHeader.UserId);

		//		cartFromDb.CouponCode = "";

		//		appDbContext.CartHeaders.Update(cartFromDb);
		//		await appDbContext.SaveChangesAsync();
		//		_responseDto.Result = true;
		//	}
		//	catch (Exception ex)
		//	{

		//		_responseDto.Message = ex.Message;
		//		_responseDto.IsSuccess = false;
		//	}
		//	return _responseDto;
		//}


		[HttpGet("GetCart/{userId}")]
		public async Task<ResponseDto> GetCart(string userId)
		{
			try
			{
				CartDto cartDto = new CartDto()
				{
					CartHeader = mapper.Map<CartHeaderDto>(await appDbContext.CartHeaders.AsNoTracking()
					.FirstOrDefaultAsync(u => u.UserId == userId))
				};


				cartDto.CartDetails = mapper.Map<IEnumerable<CartDetailsDto>>(appDbContext.CartDetails
					.AsNoTracking()
					.Where(u => u.CartHeaderId == cartDto.CartHeader.CartHeaderId));


				IEnumerable<ProductDto> productDtos = await productService.GetProducts();


				foreach (var item in cartDto.CartDetails)
				{
					item.Product = productDtos.FirstOrDefault(u => u.ProductId == item.ProductId);
					cartDto.CartHeader.CartTotal += (item.Count * item.Product.Price);
				}

				//apply coupon if any
				if (!string.IsNullOrEmpty(cartDto.CartHeader.CouponCode))
				{
					CouponDto coupon = await couponService.GetCoupon(cartDto.CartHeader.CouponCode);
					if(coupon is not null && cartDto.CartHeader.CartTotal > coupon.MinAmount)
					{
						cartDto.CartHeader.CartTotal -= coupon.DiscountAmount;
						cartDto.CartHeader.Discount = coupon.DiscountAmount;
					}
				}

				_responseDto.Result = cartDto;
			}
			catch (Exception ex)
			{

				_responseDto.Message = ex.Message;
				_responseDto.IsSuccess = false;
			}
			return _responseDto;
		}


		[HttpPost("CartUpsert")]
		public async Task<ResponseDto> CartUpsert([FromBody]CartDto cartDto)
		{
			try
			{
				var cartHeaderDromDb = await appDbContext.CartHeaders.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == cartDto.CartHeader.UserId);
				if(cartHeaderDromDb is null)
				{
					//create CartHeader and Insert Details

					CartHeader cartHeader = mapper.Map<CartHeader>(cartDto.CartHeader);
					appDbContext.CartHeaders.Add(cartHeader);

					 await appDbContext.SaveChangesAsync();

					cartDto.CartDetails.First().CartHeaderId = cartHeader.CartHeaderId;
					appDbContext.CartDetails.Add(mapper.Map<CartDetails>(cartDto.CartDetails.First()));

					await appDbContext.SaveChangesAsync();
				}
				else
				{
					//if header is not null
					//check if details has same product
					var cartDetailsFromDb = await appDbContext.CartDetails.AsNoTracking().FirstOrDefaultAsync(
						u => u.ProductId == cartDto.CartDetails.First().ProductId && 
						u.CartHeaderId == cartHeaderDromDb.CartHeaderId);
					if(cartDetailsFromDb is null)
					{
						//create cartDetails
						cartDto.CartDetails.First().CartHeaderId = cartHeaderDromDb.CartHeaderId;
						appDbContext.CartDetails.Add(mapper.Map<CartDetails>(cartDto.CartDetails.First()));

						await appDbContext.SaveChangesAsync();

					}
					else
					{
						//update count in cart details
						cartDto.CartDetails.First().Count += cartDetailsFromDb.Count;
						cartDto.CartDetails.First().CartHeaderId = cartDetailsFromDb.CartHeaderId;
						cartDto.CartDetails.First().CartDetailsId = cartDetailsFromDb.CartDetailsId;

						appDbContext.CartDetails.Update(mapper.Map<CartDetails>(cartDto.CartDetails.First()));

						await appDbContext.SaveChangesAsync();
					}
				}

				_responseDto.Result = cartDto;
			}
			catch (Exception ex)
			{

				_responseDto.Message = ex.Message;
				_responseDto.IsSuccess = false;
			}
			return _responseDto;
		}



		[HttpPost("CartRemove")]
		public async Task<ResponseDto> CartRemove([FromBody] int cartDetailsId)
		{
			try
			{
				
				CartDetails cartDetails = appDbContext.CartDetails
					.First(u => u.CartDetailsId == cartDetailsId);

				int totalCountofCartItem = appDbContext.CartDetails.AsNoTracking()
					.Where(c => c.CartHeaderId == cartDetails.CartHeaderId)
					.Count();

				appDbContext.CartDetails.Remove(cartDetails);

				if(totalCountofCartItem == 1)
				{
					var cartHeaderFromDbToRemove = await appDbContext.CartHeaders.AsNoTracking()
						.FirstOrDefaultAsync(u => u.CartHeaderId == cartDetails.CartHeaderId);

					appDbContext.CartHeaders.Remove(cartHeaderFromDbToRemove);
				}

				await appDbContext.SaveChangesAsync();
				_responseDto.Result = true;
			}
			catch (Exception ex)
			{

				_responseDto.Message = ex.Message;
				_responseDto.IsSuccess = false;
			}
			return _responseDto;
		}


	}
}
