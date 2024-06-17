using Microsoft.EntityFrameworkCore;
using Mango.Services.RewardApi.Data;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//adding a db as inside  a singleton 
var optionBuilder = new DbContextOptionsBuilder<AppDbContext>();

optionBuilder.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
//inside service we will create our own DbCOntext because di impossible to add scoped serv inside singleton


//builder.Services.AddSingleton(new EmailService(optionBuilder.Options));




builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});


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
ApplyMigration();
app.Run();

void ApplyMigration()
{
    using (var scope = app.Services.CreateScope())
    {
        var _db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        if (_db.Database.GetPendingMigrations().Count() > 0) // 
        {
            _db.Database.Migrate();
        }
    }
}

