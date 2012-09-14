using SignalR.Client.Hubs;
using SignalR.Client.Transports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SignalRTest
{
    class Program
    {
        static int count = 0;

        static void Main(string[] args)
        {
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;

            Console.WriteLine("Press enter to start");
            Console.ReadLine();

            // Batch the connections, once they're all active though it's fine
            int connectionBatchSize = 10;
            int connectionSleepInterval = 1000;
            int maxClients = 3000;

            var options = new ParallelOptions
            {
                MaxDegreeOfParallelism = connectionBatchSize
            };

            for (int x = 0; x < maxClients; x += connectionBatchSize)
            {
                Parallel.For(0, connectionBatchSize, options, i =>
                    {
                        StartClient();
                    });

                System.Threading.Thread.Sleep(connectionSleepInterval);
            }

            Console.Read();
        }

        static void StartClient()
        {
            var hub = new HubConnection("http://localhost:49899/");
            var proxy = hub.CreateProxy("LivePricingHub");

            proxy.On("TradeOccurred", d =>
            {
                Console.WriteLine("UPDATE: {0} - {1:C} ({2:N0})", d.ShortCode, d.LastTradePrice, d.TradeVolume);
            });

            hub.Start()
                .ContinueWith(t =>
                {
                    count++;

                    for (int x = 0; x < 20; x++)
                    {
                        var listenTask = proxy.Invoke<dynamic>("ListenForPriceChanges", string.Format("Share {0}", x));
                        listenTask.ContinueWith(priceTask =>
                            {
                                var price = priceTask.Result;

                            }, TaskContinuationOptions.OnlyOnRanToCompletion);
                    }

                    Console.WriteLine("{0} Connected", count);

                }, TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        private static void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            Console.WriteLine(e.Exception.GetBaseException());
            e.SetObserved();
        }
    }
}
