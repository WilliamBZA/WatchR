using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WatchRServer.HubHelpers
{
    public class TradeOccurredEventArgs
    {
        public string ShortCode { get; set; }

        public decimal TradePrice { get; set; }

        public decimal TradeVolume { get; set; }
    }
}
