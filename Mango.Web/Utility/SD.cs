namespace Mango.Web.Utility
{
	public class SD
	{
		public static string? CouponApiBase { get; set; }
		public static string? AuthApiBase { get; set; }

		public const string RoleAdmin = "ADMIN";
		public const string RoleUser = "USER";


		public const string TokenCookie = "JWTToken";
		public enum ApiType
		{
			GET, POST, PUT, DELETE
		}

	}
}
