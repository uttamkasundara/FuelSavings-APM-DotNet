using FuelSavings.Core.Models;

namespace FuelSavings.Core.Services;

public class FuelCalculator
{
    private const double DefaultVehicleMassKg = 1500.0;
    private const double DefaultEngineEfficiency = 0.25; // fraction
    private const double FuelEnergyDensityJPerL = 34_200_000.0; // J/L for gasoline
    private const double DefaultBaseLPer100Km = 6.8;

    public FuelResponse Calculate(FuelRequest req)
    {
        var model = (req.Model ?? "physics").ToLowerInvariant();

        double baseLPer100 = req.BaseLPer100Km ?? DefaultBaseLPer100Km;
        double totalBaseL = req.DistanceKm * baseLPer100 / 100.0;

        double totalWithBadL = totalBaseL;
        double extraL = 0.0;

        if (model == "simple")
        {
            var extraPer = req.ExtraLPerAccel ?? 0.05; // default 50 ml per unnecessary accel
            extraL = req.AccelEvents * extraPer;
            totalWithBadL += extraL;
        }
        else // physics or default
        {
            double mass = req.VehicleMassKg ?? DefaultVehicleMassKg;
            double dv = req.AvgDeltaVmS ?? 2.5; // m/s
            int n = req.AccelEvents;
            double eff = req.EngineEfficiency ?? DefaultEngineEfficiency;

            if (n > 0 && dv > 0 && eff > 0)
            {
                double energyPerAccelJ = 0.5 * mass * dv * dv;
                double litersPerAccel = energyPerAccelJ / (eff * FuelEnergyDensityJPerL);
                extraL = n * litersPerAccel;
                totalWithBadL += extraL;
            }
        }

        double savingsL = totalWithBadL - totalBaseL;
        double savingsPct = totalBaseL > 0 ? (savingsL / totalBaseL) * 100.0 : 0.0;

        var resp = new FuelResponse
        {
            TotalNoStopL = Math.Round(totalBaseL, 5),
            TotalWithBadL = Math.Round(totalWithBadL, 5),
            SavingsL = Math.Round(savingsL, 5),
            SavingsPct = Math.Round(savingsPct, 3),
            Breakdown = new { base_l = Math.Round(totalBaseL, 5), extra_l = Math.Round(extraL, 5) }
        };

        return resp;
    }
}
