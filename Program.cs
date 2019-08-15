using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace SupermarketQueue
{
    class Program
    {
        static void Main(string[] args)
        {
            SuperMarketOrderProcess superMarketOrderProcess = new SuperMarketOrderProcess();
            superMarketOrderProcess.RunQueue();
            Console.ReadKey();
        }
    }
}
