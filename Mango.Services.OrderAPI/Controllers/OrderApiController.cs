using AutoMapper;
using Mango.Services.OrderAPI.Models;
using Mango.Services.OrderAPI.Models.DTO;
using Mango.Services.OrderAPI.Services.IService;
using Mango.Services.OrderAPI.Utilities;
using Mango.Services.ShopingCartApi.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.OrderAPI.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderApiController : ControllerBase
    {
        private readonly AppDbContext appDbContext;
        private readonly IProductService productService;
        private readonly IMapper mapper;
        private readonly ResponseDto response;

        public OrderApiController(AppDbContext appDbContext, IProductService productService, IMapper mapper)
        {
            this.appDbContext = appDbContext;
            this.productService = productService;
            this.mapper = mapper;

            response = new ResponseDto();
        }

        [Authorize]
        [HttpPost("CreateOrder")]
        public async Task<ResponseDto> CreateOrder([FromBody]CartDto cartDto)
        {
            try
            {
                OrderHeaderDto orderHeaderDto = mapper.Map<OrderHeaderDto>(cartDto.CartHeader);
                orderHeaderDto.OrderTime = DateTime.Now;
                orderHeaderDto.Status = SD.Status_Pending;
                orderHeaderDto.OrderDetails = mapper.Map<IEnumerable<OrderDetailsDto>>(cartDto.CartDetails);

                OrderHeader orderCreated = appDbContext.OrderHeaders.Add(mapper.Map<OrderHeader>(orderHeaderDto)).Entity;
                await appDbContext.SaveChangesAsync();

                orderHeaderDto.OrderHeaderId = orderCreated.OrderHeaderId;
                response.Result = orderHeaderDto;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }
    }
}
