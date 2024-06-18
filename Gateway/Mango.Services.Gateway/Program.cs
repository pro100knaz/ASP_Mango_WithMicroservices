using Mango.GatewaySolution.Extensions;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

//Register Ocelot
builder.Services.AddOcelot();

//Authentification inside extensions by JwtBearer as inside otheer services
builder.AddAppAuthentication();

var app = builder.Build();



app.MapGet("/", () => "Hello World!");

await app.UseOcelot();

app.Run();
