using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Linq;
using System;
using System.Text;

namespace ExchangeRatesLib
{
    public class Currency
    {
        public string Code { get; set; }
        public string FullName { get; set; }

        public Currency(string code, string fullName = null)
        {
            Code = code;
            FullName = fullName;
        }
    }
}