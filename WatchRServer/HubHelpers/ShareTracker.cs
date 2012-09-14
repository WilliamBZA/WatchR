using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Web;
using WatchRServer.Models;

namespace WatchRServer.HubHelpers
{
    public class ShareTracker : IDisposable
    {
        private Share _share;
        private Timer _timer;

        public delegate void TradeOccurredHandler(object sender, TradeOccurredEventArgs eventArgs);
        public event TradeOccurredHandler TradeOccurred;

        public ShareTracker(Share share, int updateInterval)
        {
            if (share == null)
            {
                throw new ArgumentNullException("share");
            }

            _share = share;

            Clients = new ConcurrentBag<string>();

            _timer = new Timer(updateInterval);
            _timer.Elapsed += (s, e) =>
                {
                    CheckForShareUpdates();
                };

            _timer.Start();
        }

        public ShareTracker(Share share)
            : this(share, 1000)
        {
        }

        public ConcurrentBag<string> Clients { get; private set; }

        public Share Share
        {
            get { return _share; }
        }

        private void CheckForShareUpdates()
        {
            // Updates are simulated now, so whatever.
            // For prod code this should be mockable

            Random random = new Random();
            _share.LastTradePrice += (decimal)(random.NextDouble() - 0.5);
            _share.TradeVolume = random.Next(0, 100000000);

            if (TradeOccurred != null)
            {
                TradeOccurred(_share, new TradeOccurredEventArgs
                {
                    ShortCode = _share.ShortCode,
                    TradePrice = _share.LastTradePrice,
                    TradeVolume = _share.TradeVolume
                });
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_timer != null)
                {
                    _timer.Dispose();
                    _timer = null;
                }
            }
        }
    }
}