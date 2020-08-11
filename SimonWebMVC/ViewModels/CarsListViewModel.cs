using System;
using System.Collections.Generic;
using CL3C.Models;
using ExchangeRatesLib;
using SimonWebMVC.Models;

namespace SimonWebMVC.ViewModels
{
    public class CL3CListViewModel
    {
        public PagedList.Core.IPagedList<BaseCar> Cars { get; set; }
        public ExchangeRates ExchangeRates { get; set; }
        public CurrencyEnum Currency { get; set; }
        // public Tuple<OrderByEnum, bool> Order { get; set; }
        public string SearchString { get; set; }
        public string SortString { get; set; }
        public int PageCount { get; set; }
        public int PageNumber { get; set; }
    }
}