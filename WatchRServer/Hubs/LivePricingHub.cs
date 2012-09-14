using SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using WatchRServer.Data;
using WatchRServer.DTOs;
using WatchRServer.HubHelpers;
using WatchRServer.Models;

namespace WatchRServer.Hubs
{
    public class LivePricingHub : Hub, IDisconnect
    {
        private static Dictionary<string, ShareTracker> _shareTrackers;
        private static object _shareLock = new Object();

        public Dictionary<string, ShareTracker> SharesTracked
        {
            get
            {
                _shareTrackers = _shareTrackers ?? new Dictionary<string, ShareTracker>();

                return _shareTrackers;
            }
        }

        public PriceDTO ListenForPriceChanges(string shortCode)
        {
            ShareTracker tracker = null;

            // Check if already listening to updates for that code
            if (!SharesTracked.ContainsKey(shortCode))
            {
                lock (_shareLock)
                {
                    if (!SharesTracked.ContainsKey(shortCode))
                    {
                        var share = new Repository().GetPriceForShare(shortCode);
                        tracker = new ShareTracker(share);

                        SharesTracked.Add(shortCode, tracker);

                        // Subscribe to trades
                        tracker.TradeOccurred += tracker_TradeOccurred;
                    }
                }
            }

            tracker = SharesTracked[shortCode];

            if (!tracker.Clients.Contains(Context.ConnectionId))
            {
                // Add client ID to that code's set of listeners
                tracker.Clients.Add(Context.ConnectionId);
            }

            // Add client to that broadcast group
            Groups.Add(Context.ConnectionId, shortCode);

            // Get current price and return
            return new PriceDTO
                {
                    LastTradePrice = tracker.Share.LastTradePrice,
                    ShortCode = tracker.Share.ShortCode,
                    TradeVolume = tracker.Share.TradeVolume
                };
        }

        private void tracker_TradeOccurred(object sender, TradeOccurredEventArgs eventArgs)
        {
            Clients[eventArgs.ShortCode].TradeOccurred(new PriceDTO
                                                        {
                                                            ShortCode = eventArgs.ShortCode,
                                                            LastTradePrice = eventArgs.TradePrice,
                                                            TradeVolume = eventArgs.TradeVolume
                                                        });
        }

        public Task Disconnect()
        {
            return null;
        }
    }
}