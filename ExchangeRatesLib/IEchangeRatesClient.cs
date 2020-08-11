using System.Collections.Generic;

namespace ExchangeRatesLib
{
    public interface IExchangeRatesClient
    {
        ExchangeRates GetExchangeRates(CurrencyEnum? baseCurrency = null, List<CurrencyEnum> currenciesToRetrieve = null);
    }
}