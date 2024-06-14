using AutoMapper;
using Mango.Services.ShopingCartApi.Models;
using Mango.Services.ShopingCartApi.Models.DTO;


namespace Mango.Services.ShopingCartApi
{
	public class MappingConfig
	{
		public static MapperConfiguration RegisterMaps()
		{
			var mappingConfig = new MapperConfiguration(config =>
			{
				config.CreateMap<CartHeaderDto, CartHeader>().ReverseMap();
				config.CreateMap<CartDetailsDto, CartDetails>().ReverseMap();


			});

			return mappingConfig;
		}
	}
}
