using Mango.Services.OrderAPI.Models.DTO;
namespace Mango.Services.OrderAPI.Services.IService
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetProducts();
    }
}
