using Mango.GatewaySolution.Extensions;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

//Register Ocelot and past inside json file
builder.Configuration.AddJsonFile("Ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot(builder.Configuration);

//Authentification inside extensions by JwtBearer as inside otheer services
builder.AddAppAuthentication();

var app = builder.Build();



app.MapGet("/", () => "Hello World!");

//await app.UseOcelot();
app.UseOcelot().GetAwaiter().GetResult();
app.Run();
