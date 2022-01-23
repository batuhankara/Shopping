using Microsoft.EntityFrameworkCore;
using Shopping.Application.QueryService;
using Shopping.Application.Services;
using Shopping.Infra.Common.EventStore;
using Shopping.Persistance.GetEventStore.Repositories;
using Shopping.Persistance.ReadStore.DbContext;
using Shopping.Query;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IOrderCommandService, OrderCommandService>();
builder.Services.AddSingleton<IEventStoreConnectionManager, EventStoreConnectionManager>();
builder.Services.AddScoped<IOrderCommandService, OrderCommandService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderQueryStore, OrderQueryStore>();
builder.Services.AddScoped<IOrderQueryService, OrderQueryService>();

string sqlConnectionSring=builder.Configuration.GetConnectionString("ReadStoreConnection");
builder.Services.AddDbContext<OrderDbContext>(

        options => options.UseSqlServer(sqlConnectionSring)) ; ;
var app = builder.Build();
using (var scope = app.Services.GetService<IServiceScopeFactory>().CreateScope())
{
    scope.ServiceProvider.GetRequiredService<OrderDbContext>().Database.Migrate();
}
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
