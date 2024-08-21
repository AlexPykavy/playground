using Microsoft.EntityFrameworkCore;
using Demo.Azure.MultipleSqlServers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<WeatherMainDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Main")));
builder.Services.AddDbContext<WeatherMigrationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Migration")));
builder.Services.AddDbContext<WeatherReadOnlyDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ReadOnly")));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
