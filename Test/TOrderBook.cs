using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Orderbook.Entities;
using Orderbook.ServiceLayer;

namespace Orderbook.Test
{
    class TOrderBook
    {
        int[] clientPool = { 1, 2, 3, 4 };
        string[] tickerPool = { "AMZN", "APPL", "FB", "GOOG" };
        OrderType[] typePool = { OrderType.Buy, OrderType.Sell };

        private Random _rand;
        private OrderBook _book;

        public TOrderBook(OrderBook testObj)
        {
            _rand = new Random();
            _book = testObj;
        }

        internal Order RandomOrder()
        {
            var o = new Order
            {
                ClientID = clientPool[_rand.Next(clientPool.Length)],
                Ticker = tickerPool[_rand.Next(tickerPool.Length)],
                Amount = _rand.Next(1, 10),
                Price = Decimal.Round((decimal)_rand.Next(1, 10) + (decimal)_rand.NextDouble(), 2),
                Type = typePool[_rand.Next(typePool.Length)]
            };
            return o;
        }

        internal void Test(int numOrder)
        {
            Order temp;

            for (int _ = 0; _ < numOrder; _++)
            {
                temp = RandomOrder();
                Console.WriteLine($"------> {temp}");
                _book.AddOrder(temp);
            }

            _book.Terminate();
            Console.ReadLine();
        }
    }
}
