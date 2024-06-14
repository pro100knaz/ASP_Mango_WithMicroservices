using Mango.Services.ShopingCartApi.Models.DTO;

namespace Mango.Services.ShopingCartApi.Services.IService
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetProducts();
    }
}
