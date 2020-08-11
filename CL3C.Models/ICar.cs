namespace CL3C.Models
{
    public interface ICar : IBaseCar
    {
        ulong? Maintenance1 { get; set; }
        ulong? Maintenance2 { get; set; }
        ulong? Maintenance3 { get; set; }
        ulong? Maintenance1Price { get; set; }
        ulong? Maintenance2Price { get; set; }
        ulong? Maintenance3Price { get; set; }
        ulong? Maintenance1Years { get; set; }
        ulong? Maintenance2Years { get; set; }
        ulong? Maintenance3Years { get; set; }
        ulong? MTBF { get; set; }
        ulong? AverageRepairCosts { get; set; }
        decimal? AdvancedLifeCycleCosts { get; set; }
        decimal? AdvancedCostsPerDistanceUnit { get; set; }
    }
}