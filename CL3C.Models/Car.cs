using System;

namespace CL3C.Models
{
    public class Car : BaseCar, ICar
    {
        public ulong? Maintenance1 { get; set; }
        public ulong? Maintenance2 { get; set; }
        public ulong? Maintenance3 { get; set; }
        public ulong? Maintenance1Price { get; set; }
        public ulong? Maintenance2Price { get; set; }
        public ulong? Maintenance3Price { get; set; }
        public ulong? Maintenance1Years { get; set; }
        public ulong? Maintenance2Years { get; set; }
        public ulong? Maintenance3Years { get; set; }
        public ulong? MTBF { get; set; }
        public ulong? AverageRepairCosts { get; set; }
        public decimal? AdvancedLifeCycleCosts { get; set; }
        public decimal? AdvancedCostsPerDistanceUnit { get; set; }
    }
}
