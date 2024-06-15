using Mango.Web.Services;
using Mango.Web.Services.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor(); // it is necessary t o use cookies
builder.Services.AddHttpClient();


builder.Services.AddHttpClient<ICouponService, CouponService>();
builder.Services.AddHttpClient<IAuthService, AuthService>();
builder.Services.AddHttpClient<IProductService, ProductService>();
builder.Services.AddHttpClient<IShoppingCartService, ShoppingCartService>();




builder.Services.AddScoped<IBaseService, BaseService>();
builder.Services.AddScoped<ITokenProvider, TokenProvider>();
builder.Services.AddScoped<ICouponService, CouponService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IShoppingCartService, ShoppingCartService>();


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
	options.ExpireTimeSpan = TimeSpan.FromHours(10);
	options.LoginPath = "/Auth/Login";
	options.AccessDeniedPath = "/Auth/AccessDenied";
});


SD.CouponApiBase = builder.Configuration["ServiceUrls:CouponAPI"]!;
SD.AuthApiBase = builder.Configuration["ServiceUrls:AuthApi"]!;
SD.ProductApiBase = builder.Configuration["ServiceUrls:ProductApi"]!;
SD.ShoppingCartApiBase = builder.Configuration["ServiceUrls:ShoppingCartApi"]!;



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
