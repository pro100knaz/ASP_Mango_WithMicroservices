using Mango.Web.Models;
using Mango.Web.Models.DTO;

namespace Mango.Web.Services.IService
{
	public interface IBaseService<T>
	{
		Task<ResponseDto<T>> SendAsync(RequestDto requestDto);
	}
}
