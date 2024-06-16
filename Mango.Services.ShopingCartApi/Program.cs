using AutoMapper;
using Mango.Services.ShopingCartApi;
using Mango.Services.ShopingCartApi.Data;
using Mango.Services.ShopingCartApi.Extensions;
using Mango.Services.ShopingCartApi.Services;
using Mango.Services.ShopingCartApi.Services.IService;
using Mango.Services.ShopingCartApi.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddHttpClient("Product", u=> u.BaseAddress 
= new Uri(builder.Configuration["ServiceUrls:ProductApi"])).AddHttpMessageHandler<BackendApiAuthHttpClientHAndler>(); // to передать token for authentification

builder.Services.AddHttpClient("Coupon", u=> u.BaseAddress 
= new Uri(builder.Configuration["ServiceUrls:CouponApi"])).AddHttpMessageHandler<BackendApiAuthHttpClientHAndler>();

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICouponService, CouponService>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<BackendApiAuthHttpClientHAndler>();


builder.Services.AddSwaggerGen(options =>
{
	options.AddSecurityDefinition(name: JwtBearerDefaults.AuthenticationScheme, securityScheme: new OpenApiSecurityScheme()
	{
		Name = "Authorization",
		Description = "Enter the Beare Authorization string as following:  'Bearer GEnerated-JWT-Token'",
		In = ParameterLocation.Header,
		Type = SecuritySchemeType.ApiKey,
		Scheme = "Bearer"
	}); // default vaalue to set authentification for swagger

	options.AddSecurityRequirement(new OpenApiSecurityRequirement()
	{
		{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference
				{
					Type = ReferenceType.SecurityScheme,
					Id= JwtBearerDefaults.AuthenticationScheme
				}
			},
			new string[]{}
		}
	});
});



IMapper mapper = MappingConfig.RegisterMaps().CreateMapper();

builder.Services.AddSingleton(mapper);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddDbContext<AppDbContext>(options =>
{
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});


builder.AddAppAuthentication();

builder.Services.AddAuthentication();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
