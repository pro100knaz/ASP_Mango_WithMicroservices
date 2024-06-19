using AutoMapper;
using Mango.MessageBus;
using Mango.Services.OrderAPI.Extensions;
using Mango.Services.OrderAPI.RabbitMqSender;
using Mango.Services.OrderAPI.Services;
using Mango.Services.OrderAPI.Services.IService;
using Mango.Services.OrderAPI.Utilities;
using Mango.Services.ShopingCartApi;
using Mango.Services.ShopingCartApi.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddHttpClient("Product", u => u.BaseAddress
= new Uri(builder.Configuration["ServiceUrls:ProductApi"])).AddHttpMessageHandler<BackendApiAuthHttpClientHAndler>(); // to передать token for authentification

builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<BackendApiAuthHttpClientHAndler>();

builder.Services.AddScoped<IMessageBus, MessageBus>();
builder.Services.AddScoped<IRabbitMqOrderMessageSender, RabbitMqOrderMessageSender>();


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
	c.SwaggerEndpoint("/swagger/v1/swagger.json", "Order API");
	c.RoutePrefix = string.Empty;
});

Stripe.StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe").Get<string>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
