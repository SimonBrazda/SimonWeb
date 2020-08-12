using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CL3C.Models;
using CL3C.Services;
using ExchangeRatesLib;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SimonWebMVC.Security;
using SimonWebMVC.ViewModels;
using SimonWebMVC.Extensions;
using SimonWebMVC.Models;
using PagedList.Core;
using reCAPTCHA.AspNetCore.Attributes;
using Microsoft.Extensions.Logging;

namespace SimonWebMVC.Controllers.CL3C
{
    public class CL3CController : Controller
    {
        private readonly ICarRepo carRepo;
        private readonly IExchangeRatesClient exchangeRatesClient;
        private readonly IAuthorizationService authorizationService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger<CL3CController> logger;

        public CL3CController(ICarRepo carRepo, IExchangeRatesClient exchangeRatesClient, IAuthorizationService authorizationService, UserManager<ApplicationUser> userManager, ILogger<CL3CController> logger)
        {
            this.logger = logger;
            this.userManager = userManager;
            this.authorizationService = authorizationService;
            this.carRepo = carRepo;
            this.exchangeRatesClient = exchangeRatesClient;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ViewResult> List(string sortString, string searchString, CurrencyEnum? currency, int? page)
        {
            var user = await userManager.GetUserAsync(HttpContext.User);
            IQueryable<BaseCar> cars;

            CL3CListViewModel carsListViewModel = new CL3CListViewModel()
            {
                ExchangeRates = exchangeRatesClient.GetExchangeRates(null, new List<CurrencyEnum> { CurrencyEnum.USD, CurrencyEnum.CZK }),
                SearchString = searchString,
                SortString = sortString,
                // Currency = currency
            };

            if (String.IsNullOrEmpty(searchString) == false)
            {
                cars = carRepo.GetCarsByName(searchString);
            }
            else
            {
                cars = carRepo.GetAllCars();
            }

            switch (sortString)
            {
                case "Id_desc":
                    cars = cars.OrderByDescending(c => c.ID);
                    break;
                case "Name_asc":
                    cars = cars.OrderBy(c => c.Name);
                    break;
                case "Name_desc":
                    cars = cars.OrderByDescending(c => c.Name);
                    break;
                case "Owner_asc":
                    cars = cars.OrderBy(c => c.Owner);
                    break;
                case "Owner_desc":
                    cars = cars.OrderByDescending(c => c.Owner);
                    break;
                case "CL3C_asc":
                    cars = cars.OrderBy(c => c.BaseLifeCycleCosts);
                    break;
                case "CL3C_desc":
                    cars = cars.OrderByDescending(c => c.BaseLifeCycleCosts);
                    break;
                case "CL3CPerDistanceUnit_asc":
                    cars = cars.OrderBy(c => c.BaseCostsPerDistanceUnit);
                    break;
                case "CL3CPerDistanceUnit_desc":
                    cars = cars.OrderByDescending(c => c.BaseCostsPerDistanceUnit);
                    break;
                default:
                    cars = cars.OrderBy(c => c.ID);
                    break;
            }

            carsListViewModel.Cars = cars.ToPagedList(page ?? 1, Constants.PageSize);

            if (currency == null)
            {
                if (user != null)
                {
                    carsListViewModel.Currency = user.DefaultCurrency.ToEnum<CurrencyEnum>();
                }
                else
                {
                    carsListViewModel.Currency = Constants.DefaultCurrency;
                }
            }
            else
            {
                carsListViewModel.Currency = currency.Value;
            }

            var conversionRate = carsListViewModel.ExchangeRates.GetConversionRate(CurrencyEnum.EUR, carsListViewModel.Currency);

            foreach (var car in carsListViewModel.Cars)
            {
                car.PurchasePrice = (ulong)Math.Round(car.PurchasePrice * conversionRate);
                car.FuelPrice = (ulong)Math.Round(car.FuelPrice * conversionRate);
                car.CalcBaseLCC();
                car.CalcBaseCostsPerDistanceUnit();
            }

            return View(carsListViewModel);
        }

        [AllowAnonymous]
        [HttpPost]
        public ViewResult List(CL3CListViewModel carsListViewModel)
        {

            carsListViewModel.ExchangeRates = exchangeRatesClient.GetExchangeRates(null, new List<CurrencyEnum> { CurrencyEnum.USD, CurrencyEnum.CZK });
            var conversionRate = carsListViewModel.ExchangeRates.GetConversionRate(CurrencyEnum.EUR, carsListViewModel.Currency);

            var cars = carRepo.GetAllCars();

            if (!String.IsNullOrEmpty(carsListViewModel.SearchString))
            {
                cars = cars.Where(c => c.Name.ToLower().Contains(carsListViewModel.SearchString.ToLower())).AsQueryable();
            }

            foreach (var car in cars)
            {
                car.PurchasePrice = (ulong)Math.Round(car.PurchasePrice * conversionRate);
                car.FuelPrice = (ulong)Math.Round(car.FuelPrice * conversionRate);
                car.CalcBaseLCC();
                car.CalcBaseCostsPerDistanceUnit();
            }

            carsListViewModel.Cars = cars.ToPagedList(1, Constants.PageSize);

            return View(carsListViewModel);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult About()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var cL3CCreateOrUpdateVM = new CL3CCreateOrEditViewModel()
            {
                Car = new BaseCar(),
                Action = "Create"
            };

            var user = await userManager.GetUserAsync(User);

            cL3CCreateOrUpdateVM.Currency = user == null ?  CurrencyEnum.EUR : user.DefaultCurrency.ToEnum<CurrencyEnum>();

            var authorizationResult = await authorizationService.AuthorizeAsync(User, cL3CCreateOrUpdateVM.Car, CarOperations.Create);

            if (authorizationResult.Succeeded)
            {
                return View(cL3CCreateOrUpdateVM);
            }
            else
            {
                return RedirectToAction("AccessDenied", "Account");
            }
        }

        [HttpPost]
        [ValidateRecaptcha]
        public async Task<IActionResult> Create(CL3CCreateOrEditViewModel model)
        {
            var authorizationResult = await authorizationService.AuthorizeAsync(User, model.Car, CarOperations.Create);
            var user = await userManager.GetUserAsync(User);

            if (authorizationResult.Succeeded)
            {
                if (ModelState.IsValid)
                {
                    model.Car.Owner = user.UserName;
                    if (model.Currency != Constants.DefaultCurrency)
                    {
                        var ExchangeRates = exchangeRatesClient.GetExchangeRates(null, new List<CurrencyEnum> { CurrencyEnum.USD, CurrencyEnum.CZK });
                        var conversionRate = ExchangeRates.GetConversionRate(model.Currency, CurrencyEnum.EUR);
                        model.Car.PurchasePrice = (ulong)Math.Round(model.Car.PurchasePrice * conversionRate);
                        model.Car.FuelPrice = (ulong)Math.Round(model.Car.FuelPrice * conversionRate);
                    }
                    model.Car.CalcBaseLCC();
                    model.Car.CalcBaseCostsPerDistanceUnit();
                    model.Car.CalcBaseLCC();
                    model.Car.CalcBaseCostsPerDistanceUnit();
                    BaseCar newCar = carRepo.Add(model.Car);
                    logger.LogInformation("User of Id: {Id} and name: {Name} has created car of Id: {CarId} and name: {CarName} at {Time}.", user.Id, user.UserName, model.Car.ID, model.Car.Name, DateTime.Now);
                    return RedirectToAction(nameof(List));
                }

                return View(model);
            }

            return RedirectToAction("AccessDenied", "Account");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(ulong id)
        {
            var cL3CCreateOrUpdateVM = new CL3CCreateOrEditViewModel()
            {
                Action = "Edit",
                Car = carRepo.GetCar(id)
            };

            if (cL3CCreateOrUpdateVM.Car == null)
            {
                return RedirectToAction(nameof(CarNotFound));
            }

            var user = await userManager.GetUserAsync(User);
            cL3CCreateOrUpdateVM.Currency = user.DefaultCurrency.ToEnum<CurrencyEnum>();

            var authorizationResult = await authorizationService.AuthorizeAsync(User, cL3CCreateOrUpdateVM.Car, CarOperations.Edit);

            if (authorizationResult.Succeeded)
            {
                return View("Edit", cL3CCreateOrUpdateVM);
            }
            else
            {
                return RedirectToAction("AccessDenied", "Account");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CL3CCreateOrEditViewModel model)
        {
            var authorizationResult = await authorizationService.AuthorizeAsync(User, model.Car, CarOperations.Edit);
            var user = await userManager.GetUserAsync(User);

            if (authorizationResult.Succeeded)
            {
                if (ModelState.IsValid)
                {
                    if (model.Currency != Constants.DefaultCurrency)
                    {
                        var ExchangeRates = exchangeRatesClient.GetExchangeRates(null, new List<CurrencyEnum> { CurrencyEnum.USD, CurrencyEnum.CZK });
                        var conversionRate = ExchangeRates.GetConversionRate(model.Currency, CurrencyEnum.EUR);
                        model.Car.PurchasePrice = (ulong)Math.Round(model.Car.PurchasePrice * conversionRate);
                        model.Car.FuelPrice = (ulong)Math.Round(model.Car.FuelPrice * conversionRate);
                    }
                    BaseCar updatedCar = carRepo.Update(model.Car);
                    logger.LogInformation("User of Id: {Id} and name: {Name} has edited car of Id: {CarId} and name: {CarName} at {Time}.", user.Id, user.UserName, model.Car.ID, model.Car.Name, DateTime.Now);
                    return RedirectToAction(nameof(List));
                }

                return View(model);
            }

            return RedirectToAction("AccessDenied", "Account");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(ulong id)
        {
            BaseCar carToDelete = carRepo.GetCar(id);

            var authorizationResult = await authorizationService.AuthorizeAsync(User, carToDelete, CarOperations.Delete);

            if (carToDelete == null)
            {
                return RedirectToAction(nameof(CarNotFound), id);
            }

            if (authorizationResult.Succeeded)
            {
                return View(carToDelete);
            }

            return RedirectToAction("AccessDenied", "Account");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(BaseCar car)
        {
            var authorizationResult = await authorizationService.AuthorizeAsync(User, car, CarOperations.Delete);
            var user = await userManager.GetUserAsync(User);

            if (authorizationResult.Succeeded)
            {
                BaseCar deletedCar = carRepo.Delete(car.ID);
                logger.LogInformation("User of Id: {Id} and name: {Name} has deleted car of Id: {CarId} and name: {CarName} at {Time}.", user.Id, user.UserName, deletedCar.ID, deletedCar.Name, DateTime.Now);

                if (deletedCar == null)
                {
                    return RedirectToPage(nameof(CarNotFound), car.ID);
                }

                return RedirectToAction(nameof(List));
            }

            return RedirectToAction("AccessDenied", "Account");
        }

        [HttpGet]
        public ViewResult CarNotFound(ulong id)
        {
            Response.StatusCode = 404;
            return View();
        }
    }
}
