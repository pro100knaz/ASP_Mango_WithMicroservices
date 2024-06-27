using AutoMapper;
using Mango.Services.ProductApi.Data;
using Mango.Services.ProductApi.Models;
using Mango.Services.ProductApi.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ProductApi.Controllers
{
	[Route("api/product")]
	[ApiController]
	public class ProductApiController : ControllerBase
	{
		private readonly IMapper mapper;
		private readonly AppDbContext appDbContext;
		private ResponseDto ResponseDto;
		public ProductApiController(IMapper mapper, AppDbContext appDbContext)
		{
			this.mapper = mapper;
			this.appDbContext = appDbContext;
			ResponseDto = new ResponseDto();
		}

		[HttpGet]
		public ResponseDto Get()
		{

			try
			{
				IEnumerable<Product> objList = appDbContext.Products.ToList();
				ResponseDto.Result = mapper.Map<IEnumerable<ProductDto>>(objList);
			}
			catch (Exception ex)
			{
				ResponseDto.Message = ex.Message;
				ResponseDto.IsSuccess = false;
				//throw;
			}

			return ResponseDto;

		}

		[HttpGet]
		[Route("{id:int}")]
		public ResponseDto Get(int id)
		{
			try
			{
				Product product = appDbContext.Products.First(c => c.ProductId == id); //can use FirstOrDefault cause it doesn't trow an exception

				ResponseDto.Result = mapper.Map<ProductDto>(product);
			}
			catch (Exception ex)
			{
				ResponseDto.Message = ex.Message;
				ResponseDto.IsSuccess = false;
				//throw;
			}

			return ResponseDto;

		}

		[HttpGet]
		[Route("GetByName/{name}")]
		public ResponseDto GetByName(string name)
		{
			try
			{
				var products = appDbContext.Products
	.Where(c => c.Name.Contains(name))
	.ToList();

				if (products == null)
					ResponseDto.IsSuccess = false;
				ResponseDto.Result = mapper.Map<List<ProductDto>>(products);
			}
			catch (Exception ex)
			{
				ResponseDto.Message = ex.Message;
				ResponseDto.IsSuccess = false;
				//throw;
			}
			return ResponseDto;
		}
		[HttpPost]
		[Authorize(Roles = "ADMIN")]
		public ResponseDto Post([FromForm] ProductDto productDto) //FromForm to handle with multipart data
		{
			try
			{
				Product? product = mapper.Map<Product>(productDto);
				if (product == null)
					ResponseDto.Result = false;
				appDbContext.Products.Add(product);
				appDbContext.SaveChanges();


				//saving image 
				if (productDto.Image != null)
				{
					//i am going to create my own name for images
					string fileName = product.ProductId + Path.GetExtension(productDto.Image.FileName);
					string filePath = @$"wwwroot\ProductImages\" + fileName;

					var filePAthDirectory = Path.Combine(Directory.GetCurrentDirectory(), filePath);


					using (var fileStream = new FileStream(filePAthDirectory, FileMode.Create))
					{
						productDto.Image.CopyTo(fileStream);
					}
					// https :// {123123} /adawdadawd/ 
					var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";

					product.ImageUrl = baseUrl + @"/ProductImages/" + fileName;
					product.ImageLocalPath = filePAthDirectory;
				}
				else
				{
					product.ImageUrl = "https://placehold.co/600x400";
				}
				appDbContext.Products.Update(product);
				appDbContext.SaveChanges();

				ResponseDto.Result = mapper.Map<ProductDto>(product);
			}
			catch (Exception ex)
			{
				ResponseDto.Message = ex.Message;
				ResponseDto.IsSuccess = false;
				//throw;-
			}

			return ResponseDto;

		}

		[HttpPut]
		[Authorize(Roles = "ADMIN")]
		public ResponseDto Put([FromForm] ProductDto productDto)
		{
			try
			{
				Product? product = mapper.Map<Product>(productDto);

				if (product == null)
				{
					ResponseDto.Result = false;
					throw new ArgumentNullException();

				}

				if (productDto.Image != null)
				{

					if (!string.IsNullOrEmpty(product.ImageLocalPath))
					{
						var oldFilePAthDirectory = Path.Combine(Directory.GetCurrentDirectory(), product.ImageLocalPath);
						FileInfo file = new FileInfo(oldFilePAthDirectory);
						if (file.Exists)
						{
							file.Delete();
						}
					}



					//i am going to create my own name for images
					string fileName = product.ProductId + Path.GetExtension(productDto.Image.FileName);
					string filePath = @$"wwwroot\ProductImages\" + fileName;

					var filePAthDirectory = Path.Combine(Directory.GetCurrentDirectory(), filePath);


					using (var fileStream = new FileStream(filePAthDirectory, FileMode.Create))
					{
						productDto.Image.CopyTo(fileStream);
					}
					// https :// {123123} /adawdadawd/ 
					var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";

					product.ImageUrl = baseUrl + @"/ProductImages/" + fileName;
					product.ImageLocalPath = filePAthDirectory;
				}
				else
				{
					product.ImageUrl = "https://placehold.co/600x400";
				}

				appDbContext.Products.Update(product);
				appDbContext.SaveChanges();

				ResponseDto.Result = mapper.Map<ProductDto>(product);
			}
			catch (Exception ex)
			{
				ResponseDto.Message = ex.Message;
				ResponseDto.IsSuccess = false;
				//throw;-
			}

			return ResponseDto;

		}

		[HttpDelete]
		[Route("{id:int}")]
		[Authorize(Roles = "ADMIN")]
		public ResponseDto Delete(int id)
		{
			try
			{

				Product? product = appDbContext.Products.FirstOrDefault(c => c.ProductId == id); //can use FirstOrDefault cause it doesn't trow an exception


				if (!string.IsNullOrEmpty(product.ImageLocalPath))
				{
					var oldFilePAthDirectory = Path.Combine(Directory.GetCurrentDirectory(), product.ImageLocalPath);
					FileInfo file = new FileInfo(oldFilePAthDirectory);
					if (file.Exists)
					{
						file.Delete();
					}
				}


				var x = appDbContext.Products.Remove(product);
				appDbContext.SaveChanges();
			}
			catch (Exception ex)
			{
				ResponseDto.Message = ex.Message;
				ResponseDto.IsSuccess = false;
				//throw;-
			}

			return ResponseDto;

		}

	}
}
