namespace FuelSavings.Core.Models;

public class FuelResponse
{
    public double TotalNoStopL { get; set; }
    public double TotalWithBadL { get; set; }
    public double SavingsL { get; set; }
    public double SavingsPct { get; set; }
    public object? Breakdown { get; set; }
}
