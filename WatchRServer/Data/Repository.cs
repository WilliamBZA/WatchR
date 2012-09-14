using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Web;
using WatchRServer.Models;

namespace WatchRServer.Data
{
    public class Repository
    {
        private static Dictionary<string, Share> _lastPrices;

        protected Dictionary<string, Share> LastPrices
        {
            get
            {
                _lastPrices = _lastPrices ?? new Dictionary<string, Share>();
                return _lastPrices;
            }
        }

        public Share GetPriceForShare(string shortCode)
        {
            var random = new Random();

            if (!LastPrices.ContainsKey(shortCode))
            {
                var share = new Share
                                            {
                                                LastTradePrice = random.Next(0, 100000) / 100.0m,
                                                ShortCode = shortCode,
                                                TradeVolume = random.Next(0, 100000000),
                                            };

                LastPrices.Add(shortCode, share);

                var timer = new Timer(1000);
                timer.Elapsed += (s, e) =>
                    {
                        
                    };
            }

            return LastPrices[shortCode];
        }
    }
}