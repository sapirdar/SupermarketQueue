using System;
using System.Collections.Concurrent;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace SupermarketQueue
{
    public class SuperQueue
    {
        // Get configurations  / set defaults: 
        private readonly int CashDeskCount = ConfigurationManager.AppSettings["CashDeskCount"] != null ?
            Convert.ToInt32(ConfigurationManager.AppSettings["CashDeskCount"]) : 5;

        private readonly int OrderMinProcessTime = ConfigurationManager.AppSettings["OrderMinProcessTime"] != null ?
            Convert.ToInt32(ConfigurationManager.AppSettings["OrderMinProcessTime"]) : 1000;

        private readonly int OrderMaxProcessTime = ConfigurationManager.AppSettings["OrderMaxProcessTime"] != null ?
            Convert.ToInt32(ConfigurationManager.AppSettings["OrderMaxProcessTime"]) : 5000;

        private readonly int CustomersLineArrivalRate = ConfigurationManager.AppSettings["CustomersLineArrivalRate"] != null ?
            Convert.ToInt32(ConfigurationManager.AppSettings["CustomersLineArrivalRate"]) : 1000; // 

        private ConcurrentQueue<string> queue = new ConcurrentQueue<string>(); // Customers Id queue

        public void RunQueue()
        {
            try
            {
                // An action to proccess orders in cash desk
                Action processOrdersAction = () =>
                {
                    ProcessOrders();
                };

                // An action to add customers to the line by defined rate
                Action processCustomerQueueAction = () =>
                {
                    ProcessCustomerQueue();
                };

                // Start the 2 concurrent actions
                Parallel.Invoke(processOrdersAction, processCustomerQueueAction);
            }
            catch (Exception)
            {
                //TODO: log error
                Console.WriteLine("An error as occured");
            }
        }


        // Parallel cashiers order proccess (CashDeskCount)
        private void ProcessOrders()
        {
            Parallel.For(0, CashDeskCount, index =>
            {
                Console.WriteLine($"Cashiers No: {index} Started");

                SingleOrderProcess();
            });
        }

        // Add customers to line accordind to the defined rate
        private void ProcessCustomerQueue()
        {
            System.Timers.Timer timer = new System.Timers.Timer
            {
                Interval = CustomersLineArrivalRate,
            };

            timer.Elapsed += OnCustomerQueueTimerEvent;

            //Start the timer
            timer.Enabled = true;
        }

        private void SingleOrderProcess()
        {
            // Keep the queue consumer alive
            while (true)
            {
                // try pulling customer from the queue
                if (queue.TryDequeue(out string localCustomerId))
                {
                    Console.WriteLine($"Order process started, Customer: {localCustomerId}, Queue size: {queue.Count}");

                    // Create random number between OrderMinProcessTime and OrderMaxProcessTime
                    Random rand = new Random();
                    int processTime = rand.Next(OrderMinProcessTime, OrderMaxProcessTime);

                    // Wait for order process to complete
                    Thread.Sleep(processTime);

                    Console.WriteLine($"Order process complete, Customer: {localCustomerId}, Queue size: {queue.Count}");
                }
            }
        }

        private void OnCustomerQueueTimerEvent(object sender, ElapsedEventArgs e)
        {
            // Add customer to queue
            queue.Enqueue(Guid.NewGuid().ToString());

            Console.WriteLine($"Customer was added to the queue, Count: {queue.Count}");
        }
    }
}
