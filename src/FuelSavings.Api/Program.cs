using FuelSavings.Core.Models;
using FuelSavings.Core.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<FuelCalculator>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/api/v1/fuel/savings", (FuelRequest req, FuelCalculator calc) =>
{
    if (req.DistanceKm <= 0)
        return Results.BadRequest(new { error = "distance_km must be > 0" });

    var res = calc.Calculate(req);
    return Results.Ok(res);
}).WithName("CalculateFuelSavings");

app.Run();
