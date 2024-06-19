using AutoMapper;
using Mango.Services.ProductApi;
using Mango.Services.ProductApi.Data;
using Mango.Services.ProductApi.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

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
app.UseSwaggerUI(c =>
{
	c.SwaggerEndpoint("/swagger/v1/swagger.json", "Product API");
	c.RoutePrefix = string.Empty;
});


app.UseHttpsRedirection();

app.UseAuthorization();

app.UseStaticFiles();

app.MapControllers();

app.Run();
