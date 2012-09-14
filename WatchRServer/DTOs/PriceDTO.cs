using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WatchRServer.DTOs
{
    public class PriceDTO
    {
        public string ShortCode { get; set; }

        public decimal LastTradePrice { get; set; }

        public decimal TradeVolume { get; set; }
    }
}