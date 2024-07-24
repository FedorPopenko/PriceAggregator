using Microsoft.EntityFrameworkCore;
using PriceAggregator.DAL;
using PriceAggregator.Integrations;
using PriceAggregator.PeriodicJobs;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContextFactory<GamesDB>(o => o.UseInMemoryDatabase("MyDatabase"));
builder.Services.AddSingleton<IPriceSearcher, PriceSearcher>();
builder.Services.AddSingleton<ICurrencyRatesAPI, CurrencyRatesAPI>();
builder.Services.AddHostedService<NamesUpdater>();
builder.Services.AddCors(b =>
    b.AddDefaultPolicy(p =>
        p.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader()));
var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.UseCors();
app.Run();