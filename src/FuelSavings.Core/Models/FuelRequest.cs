namespace FuelSavings.Core.Models;

public class FuelRequest
{
    public double DistanceKm { get; set; }
    public string? Model { get; set; } = "physics";
    public int AccelEvents { get; set; }
    public double? AvgDeltaVmS { get; set; }
    public double? VehicleMassKg { get; set; }
    public double? BaseLPer100Km { get; set; }
    public double? EngineEfficiency { get; set; }
    public double? ExtraLPerAccel { get; set; }
}
