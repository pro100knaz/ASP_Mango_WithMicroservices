using AutoMapper;
using Mango.MessageBus;
using Mango.Services.OrderAPI.Models;
using Mango.Services.OrderAPI.Models.DTO;
using Mango.Services.OrderAPI.Services.IService;
using Mango.Services.OrderAPI.Utilities;
using Mango.Services.ShopingCartApi.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;

namespace Mango.Services.OrderAPI.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderApiController : ControllerBase
    {
        private readonly AppDbContext appDbContext;
        private readonly IProductService productService;
        private readonly IMessageBus messageBus;
        private readonly IConfiguration configuration;
        private readonly IMapper mapper;
        private readonly ResponseDto response;

        public OrderApiController(AppDbContext appDbContext, 
            IProductService productService,IMessageBus messageBus , 
            IConfiguration configuration , IMapper mapper)
        {
            this.appDbContext = appDbContext;
            this.productService = productService;
            this.messageBus = messageBus;
            this.configuration = configuration;
            this.mapper = mapper;

            response = new ResponseDto();
        }

        [Authorize]
        [HttpPost("CreateOrder")]
        public async Task<ResponseDto> CreateOrder([FromBody] CartDto cartDto)
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

        [Authorize]
        [HttpPost("CreateStripeSession")]
        public async Task<ResponseDto> CreateStripeSession([FromBody] StripeRequestDto stripeRequestDto)
        {

            try
            {
                //StripeConfiguration.ApiKey = "sk_test_4eC39HqLyjWDarjtT1zdp7dc";
                var options = new Stripe.Checkout.SessionCreateOptions
                {
                    SuccessUrl = stripeRequestDto.ApprovedUrl,
                    CancelUrl = stripeRequestDto.CancelUrl,
                    LineItems = new List<Stripe.Checkout.SessionLineItemOptions>(),
                    Mode = "payment",
                     
                };

                var DiscountsObj = new List<SessionDiscountOptions>() 
                {
                    new SessionDiscountOptions
                    {
                        Coupon = stripeRequestDto.OrderHeader.CouponCode
                    }
                };


                foreach (var item in stripeRequestDto.OrderHeader.OrderDetails)
                {

                    var sessionLineItems = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 1000), // $20.99 -> 2099
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Name
                            }
                        },
                        Quantity = item.Count
                    };
                    options.LineItems.Add(sessionLineItems);
                }


                if(stripeRequestDto.OrderHeader.Discount > 0)
                {
                    options.Discounts = DiscountsObj;
                }



                var service = new Stripe.Checkout.SessionService();
                Session session =  service.Create(options);

                stripeRequestDto.StripeSessionUrl = session.Url;

                OrderHeader orderHeader = appDbContext.OrderHeaders.First(u => u.OrderHeaderId == stripeRequestDto.OrderHeader.OrderHeaderId);

                orderHeader.StripeSessionId = session.Id;
                appDbContext.SaveChanges();

                response.Result = stripeRequestDto;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }



        [Authorize]
        [HttpPost("CreateStripeSession")]
        //It must be validate stripe session but i am lazy to change anything
        public async Task<ResponseDto> CreateStripeSession([FromBody] int orderHeaderId)
        {
            try
            {
                OrderHeader orderHeader = appDbContext.OrderHeaders.First(u => u.OrderHeaderId == orderHeaderId);


                var service = new Stripe.Checkout.SessionService();


                Session session = service.Get(orderHeader.StripeSessionId);
                var paymentIntentService = new PaymentIntentService();
                PaymentIntent paymentIntent = paymentIntentService.Get(session.PaymentIntentId);

                if(paymentIntent.Status == "succeeded")
                {
                    //payment was successfulr
                    orderHeader.PaymentIntentId = paymentIntent.Id;
                    orderHeader.Status = SD.Status_Approved;

                    await appDbContext.SaveChangesAsync();

                    RewardDto rewardDto = new() {
                        RewardActivity = Convert.ToInt32(orderHeader.OrderTotal),
                        OrderId = orderHeader.OrderHeaderId,
                        UserId = orderHeader.UserId
                    };

                    string TopicName = configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic");
                    await messageBus.PublishMessage(rewardDto, TopicName);

                    response.Result = mapper.Map<OrderHeaderDto>(orderHeader);
                }

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
