using CL3C.Models;
using ExchangeRatesLib;

namespace SimonWebMVC.ViewModels
{
    public class CL3CCreateOrEditViewModel
    {
        public BaseCar Car { get; set; }
        public CurrencyEnum Currency { get; set; }
        public string Action { get; set; }
    }
}