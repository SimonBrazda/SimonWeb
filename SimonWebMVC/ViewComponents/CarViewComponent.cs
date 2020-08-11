using CL3C.Services;
using CL3C.Models;
using Microsoft.AspNetCore.Mvc;
using ExchangeRatesLib;

namespace SimonWebMVC.ViewComponents
{
    public class CarViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke((BaseCar Car, CurrencyEnum Currency) Detail)
        {
            return View("Default", Detail);
        }
    }
}