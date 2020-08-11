using System;
using System.ComponentModel.DataAnnotations;
using ExchangeRatesLib;

namespace CL3C.Models
{
    public class BaseCar : IBaseCar
    {
        public ulong ID { get; set; }

        public string Owner { get; set; }

        [Required]
        [MinLength(3, ErrorMessage = "Name minimum length is 3 characters")]
        [MaxLength(40, ErrorMessage = "Name maximum length is 20 characters")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Purchase Price")]
        [Range(1, 9999999999)]
        public ulong PurchasePrice { get; set; }

        [Required]
        [Display(Name = "Technical Life")]
        [Range(1, 999999)]
        public ulong TechnicalLife { get; set; }

        [Display(Name = "Fuel Price")]
        [RegularExpression(@"^(([1-9]\d*)|0)(.0*[0-9](0*[0-9])*)?$", ErrorMessage = "Invalid decimal value format. ")]
        public decimal FuelPrice { get; set; }

        [Required]
        [Display(Name = "Fuel Consumption")]
        [RegularExpression(@"^(([1-9]\d*)|0)(.0*[0-9](0*[0-9])*)?$", ErrorMessage = "Invalid decimal value format. ")]
        public decimal FuelConsumption { get; set; }
        public decimal BaseLifeCycleCosts { get; set; }
        public decimal BaseCostsPerDistanceUnit { get; set; }

        public void CalcBaseLCC()
        {
            BaseLifeCycleCosts = (ulong)Math.Round((PurchasePrice + FuelConsumption / 100 * FuelPrice * TechnicalLife), 2);
        }

        public void CalcBaseCostsPerDistanceUnit()
        {
            BaseCostsPerDistanceUnit = (decimal)Math.Round((BaseLifeCycleCosts / (decimal)TechnicalLife), 2);
        }
    }
}