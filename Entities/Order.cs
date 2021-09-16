using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orderbook.Entities
{
    enum OrderType
    {
        Buy,
        Sell
    }

    class Order
    {
        private bool _filled;
        internal int Amount { get; set; }
        internal decimal Price { get; set; }
        internal int ClientID { get; set; }
        internal string Ticker { get; set; }
        internal OrderType Type { get; set; }

        public Order()
        {
            _filled = false;
        }

        public override string ToString()
        {
            return $"Client {ClientID} wants to {Type.ToString().ToLower()} {Amount} shares of {Ticker} at ${Price}";
        }

        internal bool IsFilled()
        {
            return _filled;
        }

        internal void Fill()
        {
            _filled = true;
        }
    }
}
