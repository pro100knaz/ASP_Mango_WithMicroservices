namespace Mango.Services.OrderAPI.Models.DTO
{
	public class ResponseDto
	{
		public object? Result { get; set; } //response List or single element
		public bool IsSuccess { get; set; } = true; // was succesful; or not
		public string Message { get; set; } = ""; // error messages if somrthing will be wrong

	}
}
