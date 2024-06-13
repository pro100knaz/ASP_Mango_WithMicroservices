using Mango.Web.Models;
using Mango.Web.Models.DTO;

namespace Mango.Web.Services.IService
{
	public interface IProductService
	{
		Task<ResponseDto?> GetProductByNameAsync(string name);
		Task<ResponseDto?> GetAllProductsAsync();
		Task<ResponseDto?> GetProductByIdAsync(int id);
		Task<ResponseDto?> CreateProductAsync(ProductDto productDto);
		Task<ResponseDto?> UpdateProductAsync(ProductDto productDto);
		Task<ResponseDto?> DeleteProductAsync(int id);
	}
}
