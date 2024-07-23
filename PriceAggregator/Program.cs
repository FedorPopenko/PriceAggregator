using Microsoft.EntityFrameworkCore;
using PriceAggregator.DAL;
using PriceAggregator.Integrations;
using PriceAggregator.PeriodicJobs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContextFactory<GamesDB>(o => o.UseInMemoryDatabase("MyDatabase"));
builder.Services.AddSingleton<IPriceSearcher, PriceSearcher>();
builder.Services.AddSingleton<ICurrencyRatesAPI, CurrencyRatesAPI>();
builder.Services.AddHostedService<NamesUpdater>();
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