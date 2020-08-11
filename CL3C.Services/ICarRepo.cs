using System.Collections.Generic;
using System.Linq;
using CL3C.Models;

namespace CL3C.Services
{
    public interface ICarRepo
    {
        IQueryable<BaseCar> GetAllCars();
        BaseCar GetCar(ulong id);
        IQueryable<BaseCar> GetCarsByName(string searchString);
        BaseCar Update(BaseCar updatedCar);
        BaseCar Add(BaseCar newCar);
        BaseCar Delete(ulong id);
    }
}