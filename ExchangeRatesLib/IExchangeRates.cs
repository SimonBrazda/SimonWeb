using System;
using System.Collections.Generic;

namespace ExchangeRatesLib
{
    public interface IExchangeRates
    {
        CurrencyEnum Base { get; set; }
        Dictionary<CurrencyEnum, decimal> Rates { get; set; }

        decimal ConvertCurrency(CurrencyEnum fromCurrency, CurrencyEnum toCurrency, decimal value);
        List<decimal> ConvertCurrency(CurrencyEnum fromCurrency, CurrencyEnum toCurrency, List<decimal> values);
        decimal GetConversionRate(CurrencyEnum fromCurrency, CurrencyEnum toCurrency);
    }
}