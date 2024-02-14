using System.Text.Json;
using EmPortal.Repos_API;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add aspire components
builder.AddNpgsqlDbContext<TicketContext>("TicketsDb");
// builder.AddSqlServerDbContext<TicketContext>("TicketsDb");
builder.AddServiceDefaults();
builder.AddRedisDistributedCache("cache");

builder.Services.AddControllers();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapGet("/tickets", (TicketContext context) =>
{
    return context.Tickets.ToListAsync();
});

app.MapPost("/tickets", async (TicketContext context, SupportTicket ticket) =>
{
    context.Tickets.Add(ticket);
    await context.SaveChangesAsync();
});

await app.Services.ApplyMigrations();


// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
