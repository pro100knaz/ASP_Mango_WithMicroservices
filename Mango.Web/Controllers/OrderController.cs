using Mango.Web.Models;
using Mango.Web.Services.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using Mango.Web.Models.DTO;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace Mango.Web.Controllers
{
	[Authorize]
	public class OrderController : Controller
	{
		private readonly IOrderService orderService;

		public OrderController(IOrderService orderService)
		{
			this.orderService = orderService;
		}
		public IActionResult OrderIndex()
		{
			return View();
		}



		[HttpPost("CancelOrder")]
		public async Task<IActionResult> CancelOrder(int orderId)
		{
			var response = orderService.UpdateOrderStatus(orderId, SD.Status_Cancelled).GetAwaiter().GetResult();

			if (response != null && response.IsSuccess)
			{
				TempData["success"] = "Status updated successfully";
				return RedirectToAction(nameof(OrderDetail), new { orderId = orderId });

			}
			TempData["error"] = "Something Went Wrong";
			return RedirectToAction("Index", "Home");
		}


		[HttpPost("CompleteOrder")]
		public async Task<IActionResult> CompleteOrder(int orderId)
		{
			var response = orderService.UpdateOrderStatus(orderId, SD.Status_Completed).GetAwaiter().GetResult();

			if (response != null && response.IsSuccess)
			{
				TempData["success"] = "Status updated successfully";
				return RedirectToAction(nameof(OrderDetail), new { orderId = orderId });

			}
			TempData["error"] = "Something Went Wrong";
			return RedirectToAction("Index", "Home");
		}


		[HttpPost("OrderReadyForPickup")]
		public async Task< IActionResult> OrderReadyForPickup(int orderId)
		{		
			var response = orderService.UpdateOrderStatus(orderId, SD.Status_ReadyForPickup).GetAwaiter().GetResult();

			if (response != null && response.IsSuccess)
			{
				TempData["success"] = "Status updated successfully";
				return RedirectToAction(nameof(OrderDetail), new { orderId = orderId});

			}
			TempData["error"] = "Something Went Wrong";
			return RedirectToAction("Index", "Home");
		}


		public async Task<IActionResult> OrderDetail(int orderId)
		{
			OrderHeaderDto orderHeaderDto = new OrderHeaderDto();

			var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault().Value;

			var response = orderService.GetOrder(orderId).GetAwaiter().GetResult();

			if (response != null && response.IsSuccess)
			{
				orderHeaderDto = JsonConvert.DeserializeObject<OrderHeaderDto>(Convert.ToString(response.Result));

			}

			if (!User.IsInRole(SD.RoleAdmin) && userId != orderHeaderDto.UserId)
			{
				//если пользователь пыттается получить доступ не к своим заказам
				return NotFound();
			}


			return View(orderHeaderDto);
		}

		[HttpGet]
		public IActionResult GetAll(string status)
		{
			IEnumerable<OrderHeaderDto> list = new List<OrderHeaderDto>();

			string userId = "";
			if (!User.IsInRole(SD.RoleAdmin))
			{
				userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault().Value;
			}
			ResponseDto response = /*await*/ orderService.Get(userId).GetAwaiter().GetResult();

			if (response != null && response.IsSuccess)
			{
				list = JsonConvert.DeserializeObject<List<OrderHeaderDto>>(Convert.ToString(response.Result));

				switch (status)
				{
					case "approved":
						list = list.Where(u => u.Status == SD.Status_Approved);
						break;
					case "readyforpickup":
						list = list.Where(u => u.Status == SD.Status_ReadyForPickup);
						break;
					case "cancelled":
						list = list.Where(u => u.Status == SD.Status_Cancelled);
						break;
					default:
						break;
				}

			}

			return Json(new { data = list });
		}
	}
}
