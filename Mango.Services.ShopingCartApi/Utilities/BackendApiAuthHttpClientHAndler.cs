
using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Headers;

namespace Mango.Services.ShopingCartApi.Utilities
{
	public class BackendApiAuthHttpClientHAndler : DelegatingHandler //позволяет перенаправить определённые данные в другой запрос user's tyoken into another request on the client side
	{
		private readonly IHttpContextAccessor accessor;

		public BackendApiAuthHttpClientHAndler(IHttpContextAccessor accessor)
		{
			this.accessor = accessor;
		}
		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			var token = await accessor.HttpContext.GetTokenAsync("access_token");

			request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

			return await base.SendAsync(request, cancellationToken);
		}

	}
}
