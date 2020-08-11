namespace CL3C.Models
{
    public interface IBaseCar
    {
        ulong ID { get; set; }
        string Name { get; set; }
        ulong PurchasePrice { get; set; }
        ulong TechnicalLife { get; set; }
        decimal FuelPrice { get; set; }
        decimal FuelConsumption { get; set; }
        decimal BaseLifeCycleCosts { get; set; }
        decimal BaseCostsPerDistanceUnit { get; set; }

        void CalcBaseCostsPerDistanceUnit();
        void CalcBaseLCC();
    }
}