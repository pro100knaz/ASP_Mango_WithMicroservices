﻿using Mango.Web.Models;
using Mango.Web.Models.DTO;

namespace Mango.Web.Services.IService
{
	public interface IOrderService
	{
		Task<ResponseDto?> CreateOrder(CartDto cartDto);
		Task<ResponseDto?> CreateStripeSession(StripeRequestDto stripeRequestDto);
		Task<ResponseDto?> ValidateStripeSession(int orderHeaderId);
		Task<ResponseDto?> Get(string? userId);
		Task<ResponseDto?> GetOrder(int orderId);
		Task<ResponseDto?> UpdateOrderStatus(int orderId, string newStatus);

	}
}
