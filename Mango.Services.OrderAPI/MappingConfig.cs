using AutoMapper;
using Mango.Services.OrderAPI.Models;
using Mango.Services.OrderAPI.Models.DTO;



namespace Mango.Services.ShopingCartApi
{
	public class MappingConfig
	{
		public static MapperConfiguration RegisterMaps()
		{
			var mappingConfig = new MapperConfiguration(config =>
			{
				config.CreateMap<OrderHeader, OrderHeaderDto>().ReverseMap();
				config.CreateMap<OrderHeaderDto, OrderHeader>().ReverseMap();


			});

			return mappingConfig;
		}
	}
}
