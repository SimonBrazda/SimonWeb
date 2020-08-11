using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Newtonsoft.Json;

namespace ExchangeRatesLib
{
    public class ExchangeRatesClient : IExchangeRatesClient
    {
        private const string API_BASE_URL = "https://api.exchangeratesapi.io/";
        private const string API_EXCHANGE_RATES_PARAM = "latest";
        private const string API_BASE_CURRENCY_PREFIX = "?base=";
        private const string API_SYMBOLS_PREFIX = "?symbols=";

        /* ---------- Some Exchange Rates webs require uniue APIKey and also enable display of list of all available currencies ---------- */
        // private const string API_CURRENCIES_PARAM = "currencies.json";
        // private const string API_APP_ID_PREFIX = "?app_id=";
        // private string APIKey;

        // Holds the cached data for each OER-url. Each unique url holds its own cached data, 
        // so for example each url with a different base currency specified has its own cached data
        private Dictionary<string, Cache> UrlCaches;

        public ExchangeRatesClient()
        {
            UrlCaches = new Dictionary<string, Cache>();
        }

        /// <summary>
        /// Retrieve the current exchange rates 
        /// </summary>
        /// <exception cref="Exception"></exception>
        /// <exception cref="APIErrorException">Thrown when the API returns an error</exception>
        /// <returns>Exchange rates</returns>
        public ExchangeRates GetExchangeRates()
        {
            return GetExchangeRates(null, null);
        }

        /// <summary>
        /// Retrieve the current exchange rates 
        /// </summary>
        /// <param name="baseCurrency">Base currency</param>
        /// <param name="currenciesToRetrieve">List of which currencies (Currency codes) to get the exchange rates for</param>
        /// <exception cref="Exception"></exception>
        /// <exception cref="APIErrorException">Thrown when the API returns an error</exception>
        /// <returns>Exchange rates</returns>
        public ExchangeRates GetExchangeRates(CurrencyEnum? baseCurrency, List<CurrencyEnum> currenciesToRetrieve)
        {
            string url = CreateExchangeRatesUrl(baseCurrency, currenciesToRetrieve);
            string json = GetJSonResponseFromUrl(url);

            return JsonConvert.DeserializeObject<ExchangeRates>(json);
        }

        /// <summary>
        /// Send a API request to the given url and retrieve the JSon string from the page or cache response.
        /// </summary>
        /// <param name="url">API url</param>
        /// <returns>JSon</returns>
        private string GetJSonResponseFromUrl(string url)
        {
            Cache cache;

            if (UrlCaches.TryGetValue(url, out cache) == true)
            {
                cache = UrlCaches[url];
            }

            if (cache != null && cache.Date == DateTime.Today)
            {
                return cache.CachedResponseJSon;
            }
            else
            {
                // Build the HTTP request
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = WebRequestMethods.Http.Get;
                request.ContentLength = 0;

                HttpWebResponse response;
                try
                {
                    // For some HTTP status codes being returned, an exception is thrown. 
                    // Catch this exception, grab the WebResponse from the exception object and deal with the response outside the try-catch block.
                    response = (HttpWebResponse)request.GetResponse();
                }
                catch (WebException e)
                {
                    response = (HttpWebResponse)e.Response;
                }

                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:             //200
                        {
                            string json = GetResponseFromHttpWebResponse(response);

                            // Cache the response
                            cache = new Cache()
                            {
                                CachedResponseJSon = json,
                                Date = DateTime.Today
                            };

                            // Add/update the cached data for this url
                            if (UrlCaches.ContainsKey(url))
                                UrlCaches[url] = cache;
                            else
                                UrlCaches.Add(url, cache);

                            return json;
                        }
                    case HttpStatusCode.NotModified:    //304
                        {
                            // Nothing changed since last request, leave the cache intact and return the cached json.
                            // Only works if your request contains header and time of last modified cache of same url.
                            return cache.CachedResponseJSon;
                        }
                    case HttpStatusCode.BadRequest:     //400
                    case HttpStatusCode.Unauthorized:   //401
                    case HttpStatusCode.NotFound:       //404
                    case HttpStatusCode.Forbidden:      //403
                    case (HttpStatusCode)429:           //429 'Too many requests' is not included in the HttpStatusCode enum
                        {
                            string json = GetResponseFromHttpWebResponse(response);
                            ErrorMessage errorMessage = JsonConvert.DeserializeObject<ErrorMessage>(json);

                            if (UrlCaches.ContainsKey(url))
                                UrlCaches.Remove(url);

                            throw new APIErrorException(errorMessage);
                        }
                    default:
                        {
                            // if (UrlCaches.ContainsKey(url))
                            //     UrlCaches.Remove(url);
                            throw new Exception("Unexpected HTTP Status code: " + response.StatusCode);
                        }
                }
            }
        }

        private string CreateExchangeRatesUrl(CurrencyEnum? baseCurrency, List<CurrencyEnum> currenciesToRetrieve)
        {
            string url = string.Empty;
            url += API_BASE_URL;
            url += API_EXCHANGE_RATES_PARAM;

            // Indicate what the base currency of the response should be, if desired
            if (baseCurrency != null)
            {
                url += API_BASE_CURRENCY_PREFIX + baseCurrency;
            }

            // Indicate the response should only include certain currencies, if desired
            if (currenciesToRetrieve != null && currenciesToRetrieve.Count > 0)
            {
                url += API_SYMBOLS_PREFIX;

                foreach (var cur in currenciesToRetrieve)
                    url += cur.ToString() + ",";

                // Remove the comma at the very end of the url
                url = url.TrimEnd(',');
            }

            return url;
        }

        private string GetResponseFromHttpWebResponse(HttpWebResponse response)
        {
            Stream stream = response.GetResponseStream();
            byte[] buffer;
            using (MemoryStream memStream = new MemoryStream((int)response.ContentLength))
            {
                byte[] part = new byte[4096];
                int bytesRead;
                while ((bytesRead = stream.Read(part, 0, part.Length)) > 0)
                {
                    memStream.Write(part, 0, bytesRead);
                }
                buffer = memStream.ToArray();
            }

            return System.Text.Encoding.UTF8.GetString(buffer);
        }
    }
}