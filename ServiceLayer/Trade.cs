using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Orderbook.Entities;

namespace Orderbook.ServiceLayer
{
    class Trade
    {
        private int _clientID;
        private OrderBook orderBook;

        public Trade(int clientID, OrderBook conn)
        {
            _clientID = clientID;
            orderBook = conn;
        }

        internal bool AddOrder(Order o)
        {
            o.ClientID = _clientID;
            return orderBook.AddOrder(o);
        }
    }
}
