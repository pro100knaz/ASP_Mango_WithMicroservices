using Mango.Web.Models;
using Mango.Web.Models.DTO;
using Mango.Web.Services.IService;
using System.Collections;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
namespace Mango.Web.Services
{
	public class BaseService : IBaseService
	{
		private readonly IHttpClientFactory httpClientFactory;
		private readonly ITokenProvider tokenProvider;

		public BaseService(IHttpClientFactory httpClientFactory, ITokenProvider tokenProvider)
        {
			this.httpClientFactory = httpClientFactory;
			this.tokenProvider = tokenProvider;
		}
        public async Task<ResponseDto> SendAsync(RequestDto requestDto, bool withBeare = true)
		{
			try
			{
				HttpClient client = httpClientFactory.CreateClient("MangoAPI");
			HttpRequestMessage message = new();


			message.Headers.Add("Accept", "application/json");

				//token
				if (withBeare)
				{
					var token = tokenProvider.GetToken();
					message.Headers.Add("Authorization", $"Bearer {token}"); //очень важно написать именно так чтобы получить доступ(авторизироваться в удалееном микросервисе) 
				}

			message.RequestUri = new Uri(requestDto.Url);

			if(requestDto.Data != null)
			{
				message.Content = new StringContent(JsonConvert.SerializeObject(requestDto.Data), Encoding.UTF8, "application/json");
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


			
	switch (apiResponse.StatusCode)
			{
				case System.Net.HttpStatusCode.NotFound:
					return new ResponseDto { IsSuccess = false, Message = "Not Found" };
				case System.Net.HttpStatusCode.Forbidden:
					return new ResponseDto { IsSuccess = false, Message = "Acces Denied" };
				case System.Net.HttpStatusCode.Unauthorized:
					return new ResponseDto { IsSuccess = false, Message = "Unauthorized" };
				case System.Net.HttpStatusCode.InternalServerError:
					return new ResponseDto { IsSuccess = false, Message = "Internal Server Error" };
				default:

						var apiContent = await apiResponse.Content.ReadAsStringAsync();
						//byte[] byteArray = Encoding.UTF8.GetBytes(apiContent);

						//ReadOnlySpan<byte> jsonReadOnlySpan = new ReadOnlySpan<byte>(byteArray);


						//var reader = new Utf8JsonReader(apiContent);

						var apiResponseDto =  JsonConvert.DeserializeObject<ResponseDto>(apiContent);
					return apiResponseDto;
			}
			}
			catch (Exception ex)
			{
				var Dto = new ResponseDto()
				{
					Message = ex.Message,
					IsSuccess = false
				};
				return Dto;
			}

		

		}
	}
}
