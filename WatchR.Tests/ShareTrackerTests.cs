using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchRServer.HubHelpers;
using WatchRServer.Models;
using Xunit;

namespace WatchR.Tests
{
    public class ShareTrackerTests
    {
        [Fact]
        public void CreatingNewTrackerWithNullShareFails()
        {
            Assert.Throws<ArgumentNullException>(() =>
                {
                    using (ShareTracker tracker = new ShareTracker(null))
                    {
                    }
                });
        }

        [Fact]
        public void CreatingNewTrackerWithValidShareInitializesEmptyClientList()
        {
            using (ShareTracker tracker = new ShareTracker(new Share
                                                            {
                                                                ShortCode = "SOL"
                                                            }))
            {
                Assert.Equal(0, tracker.Clients.Count);
            }
        }

        [Fact]
        public void TrackerExposesInstantiatedShareAsAProperty()
        {
            Share sasol = new Share()
            {
                ShortCode = "SOL"
            };

            using (ShareTracker tracker = new ShareTracker(sasol))
            {
                Assert.True(tracker.Share == sasol);
            }
        }

        [Fact]
        public void NewTrackerStartsPollForNewPrices()
        {
            Share sasol = new Share()
            {
                ShortCode = "SOL",
                LastTradePrice = -1
            };

            using (ShareTracker tracker = new ShareTracker(sasol, 1))
            {
                System.Threading.Thread.Sleep(25);

                Assert.NotEqual(-1, tracker.Share.LastTradePrice);
            }
        }

        [Fact]
        public void TrackerExposesPriceUpdatedEventWhichNotifiesOfUpdatedPrices()
        {
            Share sasol = new Share
            {
                ShortCode = "SOL",
                LastTradePrice = -1
            };

            using (ShareTracker tracker = new ShareTracker(sasol, 1))
            {
                var eventOccured = false;

                tracker.TradeOccurred += (s, e) =>
                    {
                        eventOccured = true;
                    };

                System.Threading.Thread.Sleep(25);

                Assert.True(eventOccured);
            }
        }
    }
}