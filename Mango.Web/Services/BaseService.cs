using Mango.Web.Models;
using Mango.Web.Models.DTO;
using Mango.Web.Services.IService;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Mango.Web.Services
{
	public class BaseService<T> : IBaseService<T>
	{
		private readonly IHttpClientFactory httpClientFactory;

		public BaseService(IHttpClientFactory httpClientFactory)
        {
			this.httpClientFactory = httpClientFactory;
		}
        public async Task<ResponseDto<T>> SendAsync(RequestDto requestDto)
		{
			HttpClient client = httpClientFactory.CreateClient("MangoAPI");
			HttpRequestMessage message = new();


			message.Headers.Add("Accept", "application/json");

			//token
			message.RequestUri = new Uri(requestDto.Url);

			if(requestDto.Data != null)
			{
				message.Content = new StringContent(JsonSerializer.Serialize(requestDto)/*, Encoding.UTF8, "application/json"*/);
			}

			HttpResponseMessage? apiResponse =  null;

			switch (requestDto.ApiType)
			{
				case Utility.SD.ApiType.GET:
					message.Method = HttpMethod.Get;
					break;
				case Utility.SD.ApiType.POST:
					message.Method = HttpMethod.Post;
					break;
				case Utility.SD.ApiType.PUT:
					message.Method = HttpMethod.Put;
					break;
				case Utility.SD.ApiType.DELETE:
					message.Method = HttpMethod.Delete;
					break;
				default:
					message.Method = HttpMethod.Get;
					break;
			}



			apiResponse = await client.SendAsync(message);
			var x = new ResponseDto<T>();
			try
			{
	switch (apiResponse.StatusCode)
			{
				case System.Net.HttpStatusCode.NotFound:
					return new ResponseDto<T> { IsSuccess = false, Message = "Not Found" };
				case System.Net.HttpStatusCode.Forbidden:
					return new ResponseDto<T> { IsSuccess = false, Message = "Acces Denied" };
				case System.Net.HttpStatusCode.Unauthorized:
					return new ResponseDto<T> { IsSuccess = false, Message = "Unauthorized" };
				case System.Net.HttpStatusCode.InternalServerError:
					return new ResponseDto<T> { IsSuccess = false, Message = "Internal Server Error" };
				default: 
				var apiContent = await apiResponse.Content.ReadAsStringAsync();
					var apiResponseDto = JsonSerializer.Deserialize<ResponseDto<T>>(apiContent);
					return apiResponseDto;
			}
			}
			catch (Exception ex)
			{
				var Dto = new ResponseDto<T>()
				{
					Message = ex.Message,
					IsSuccess = false
				};
				return Dto;
			}

		

		}
	}
}
