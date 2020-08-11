using System.ComponentModel.DataAnnotations.Schema;
using ExchangeRatesLib;
using Microsoft.AspNetCore.Identity;

namespace CL3C.Models
{
    public class ApplicationUser : IdentityUser
    {
        [NotMapped] //Makes it not mapped to the database
        public string EncryptedId { get; set; }

        public string DefaultCurrency { get; set; }
    }
}