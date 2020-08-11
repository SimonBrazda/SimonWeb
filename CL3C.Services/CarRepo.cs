using System.Collections.Generic;
using System.Linq;
using CL3C.Models;
using Microsoft.Extensions.Logging;

namespace CL3C.Services
{
    public class CarRepo : ICarRepo
    {
        private readonly CL3CContext _context;
        private readonly ILogger<CL3CContext> logger;

        public CarRepo(CL3CContext context, ILogger<CL3CContext> logger)
        {
            _context = context;
            this.logger = logger;
            logger.LogDebug("NLog injected into CarRepo");
        }

        public BaseCar Add(BaseCar newCar)
        {
            newCar.ID = _context.Cars.Max(c => c.ID) + 1;
            _context.Cars.Add(newCar);
            _context.SaveChanges();
            logger.LogDebug($"Car of Id {newCar.ID} and name {newCar.Name} added to the database");
            return newCar;
        }

        public BaseCar Delete(ulong id)
        {
            var carToDelete = _context.Cars.FirstOrDefault(c => c.ID == id);

            if (carToDelete != null)
            {
                _context.Cars.Remove(carToDelete);
                _context.SaveChanges();
                logger.LogDebug($"Car of Id {carToDelete.ID} and name {carToDelete.Name} deleted from the database");
            }

            return carToDelete;
        }

        public IQueryable<BaseCar> GetAllCars()
        {
            logger.LogDebug($"Getting all cars from the database");
            return _context.Cars;
        }

        public BaseCar GetCar(ulong id)
        {
            logger.LogDebug($"Getting car of Id {id} from the database");
            return _context.Cars.FirstOrDefault(car => car.ID == id);
        }

        public IQueryable<BaseCar> GetCarsByName(string searchString)
        {
            logger.LogDebug("Getting cars by name containing string: {name} from the database", searchString);
            return _context.Cars.Where(c => c.Name.ToLower().Contains(searchString.ToLower()));
        }

        public BaseCar Update(BaseCar updatedCar)
        {
            BaseCar carToUpdate = _context.Cars.FirstOrDefault(car => car.ID == updatedCar.ID);

            carToUpdate.Name = updatedCar.Name;
            carToUpdate.PurchasePrice = updatedCar.PurchasePrice;
            carToUpdate.TechnicalLife = updatedCar.TechnicalLife;
            carToUpdate.FuelPrice = updatedCar.FuelPrice;
            carToUpdate.FuelConsumption = updatedCar.FuelConsumption;
            carToUpdate.BaseLifeCycleCosts = updatedCar.BaseLifeCycleCosts;
            carToUpdate.BaseCostsPerDistanceUnit = updatedCar.BaseCostsPerDistanceUnit;
            
            _context.SaveChanges();
            logger.LogDebug($"Updated car of Id {carToUpdate.ID} and name {carToUpdate.Name} in the database");
            return carToUpdate;
        }
    }
}