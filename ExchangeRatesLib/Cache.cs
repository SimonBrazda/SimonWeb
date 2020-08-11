using System;

namespace ExchangeRatesLib
{
    /// <summary>
    /// Class containing the cached data/response from a previous request to the API.
    /// Open Exchange Rates supports ETag (Entity Tag). This means that when made use of, the server will only give a full response if the data
    /// has actually been updated since the last request. This means the responses from the server should be cached, so the cached json can be used
    /// if the server indicated this data is still up-to-date. This will save some bandwidth.
    /// if you do not require the most up to date data and only want to update per time interval comment out "*Tag", "*Data" and make use of stored "Date".
    /// </summary>
    public class Cache
    {
        // public string CachedResponseHeaderTag { get; set; }
        // public string CachedResponseHeaderData { get; set; }
        public string url { get; set; }
        public string CachedResponseJSon { get; set; }
        public DateTime Date { get; set; }
    }
}