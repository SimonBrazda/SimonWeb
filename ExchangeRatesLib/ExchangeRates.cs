using System;
using System.Collections.Generic;

namespace ExchangeRatesLib
{
    public class ExchangeRates : IExchangeRates
    {
        public CurrencyEnum Base { get; set; }
        public Dictionary<CurrencyEnum, decimal> Rates { get; set; }

        /// <summary>
        /// Calculate and return the conversion rate from one currency to another
        /// </summary>
        /// <param name="fromCurrency">Base currency</param>
        /// <param name="toCurrency">Target/desired currency</param>
        /// <exception cref="NullReferenceException">Thrown when either or both of specified currency codes is unknown, or when </exception>
        /// <returns>Conversion rate</returns>
        public decimal GetConversionRate(CurrencyEnum fromCurrency, CurrencyEnum toCurrency)
        {
            if (Rates == null || Rates.Count == 1)
                throw new NullReferenceException("No exchange rates stored in the Rates property of this instance");

            decimal fromR, toR;

            if (fromCurrency == Base)
            {
                Rates.Add(CurrencyEnum.EUR, 1.00M);
            }
            if (!Rates.TryGetValue(fromCurrency, out fromR))
                throw new NullReferenceException("Incorrect currency code: " + fromCurrency);

            if (!Rates.TryGetValue(toCurrency, out toR))
            {
                if (toCurrency == CurrencyEnum.EUR)
                {
                    toR = 1.00M;
                    return toR / fromR;
                }
                throw new NullReferenceException("Incorrect currency code: " + toCurrency);
            }

            return toR / fromR;
        }

        /// <summary>
        /// Convert given value from one currency into another
        /// </summary>
        /// <param name="fromCurrency">Base currency</param>
        /// <param name="toCurrency">Target/desired currency</param>
        /// <param name="value">Value to be converted</param>
        /// <returns>Converted value</returns>
        public decimal ConvertCurrency(CurrencyEnum fromCurrency, CurrencyEnum toCurrency, decimal value)
        {
            return value * this.GetConversionRate(fromCurrency, toCurrency);
        }

        /// <summary>
        /// Convert given values in list from one currency into another
        /// </summary>
        /// <param name="fromCurrency">Base currency</param>
        /// <param name="toCurrency">Target/desired currency</param>
        /// <param name="values">List of values to be converted</param>
        /// <returns>List of converted values</returns>
        public List<decimal> ConvertCurrency(CurrencyEnum fromCurrency, CurrencyEnum toCurrency, List<decimal> values)
        {
            List<decimal> _returnValues = new List<decimal>(values.Count);
            decimal conversionRate = this.GetConversionRate(fromCurrency, toCurrency);

            foreach (var val in values)
                _returnValues.Add(val * conversionRate);

            return _returnValues;
        }
    }
}