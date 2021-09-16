using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Orderbook.Entities;
using Orderbook.DataAccessLayer;

namespace Orderbook.ServiceLayer
{
    class OrderQueue
    {
        private bool _stopped;
        private object _lockObj;
        private Database _conn;
        private ConcurrentQueue<Order> _queue;

        public OrderQueue(Database conn)
        {
            _conn = conn;
            _queue = new ConcurrentQueue<Order>();

            _lockObj = new Object();
        }

        internal void Start()
        {
            lock (_lockObj)
            {
                _stopped = false;
                // Thread processor = new Thread(new ThreadStart(ProcessOrderQueue));
                // processor.Start();
            }
        }

        internal void Terminate()
        {
            lock (_lockObj)
            {
                _stopped = true;
            }
        }

        internal void AddOrder(Order o)
        {
            _queue.Enqueue(o);
            ProcessOrderQueue(o);
        }

        private void ProcessOrderQueue(Order candidateOrder)
        {
            if (_stopped)
            {
                return;
            }

            // Console.WriteLine("ProcessOrderQueue");
            Order buyOrder = null;
            Order sellOrder = null;

            if (candidateOrder.Type == OrderType.Buy)
            {
                buyOrder = candidateOrder;
            }
            else if (candidateOrder.Type == OrderType.Sell)
            {
                sellOrder = candidateOrder;
            }

            Monitor.Enter(candidateOrder);

            PairOrder(ref buyOrder, ref sellOrder);

            if ((buyOrder != null) && (sellOrder != null) && (ValidateOrder(ref buyOrder, ref sellOrder)))
            {
                FillOrder(ref buyOrder, ref sellOrder);
            }

            ReleaseOrder(ref buyOrder);
            ReleaseOrder(ref sellOrder);
        }

        private void PairOrder(ref Order buyOrder, ref Order sellOrder)
        {
            // Console.WriteLine("PairOrder");
            foreach (Order o in _queue){
                if (!o.IsFilled() && !Monitor.IsEntered(o))
                {
                    Monitor.Enter(o);

                    if (o.Type == OrderType.Buy)
                    {
                        if (buyOrder == null)
                        {
                            if ((sellOrder == null || o.Price >= sellOrder.Price) && o.ClientID != sellOrder.ClientID)
                            {
                                buyOrder = o;
                            }
                            else
                            {
                                Monitor.Exit(o);
                            }
                        }
                    }
                    else if (o.Type == OrderType.Sell)
                    {
                        if (sellOrder == null)
                        {
                            if ((buyOrder == null || o.Price <= buyOrder.Price) && o.ClientID != buyOrder.ClientID)
                            {
                                sellOrder = o;
                            }
                            else
                            {
                                Monitor.Exit(o);
                            }
                        }
                    }
                    else
                    {
                        Monitor.Exit(o);
                    }
                }

                if (buyOrder != null && sellOrder != null)
                {
                    break;
                }
            }
        }

        private bool ValidateOrder(ref Order buyOrder, ref Order sellOrder)
        {
            // Console.WriteLine("ValidateOrder");
            int requiredAmount = Math.Min(buyOrder.Amount, sellOrder.Amount);
            decimal requiredBalance = sellOrder.Price * sellOrder.Amount;

            if (_conn.HasEnoughBalance(buyOrder.ClientID, requiredBalance) && _conn.HasEnoughShares(sellOrder.ClientID, buyOrder.Ticker, requiredAmount))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void FillOrder(ref Order buyOrder, ref Order sellOrder)
        {
            // Console.WriteLine("FillOrder");
            int amount = Math.Min(buyOrder.Amount, sellOrder.Amount);
            decimal price = Math.Min(buyOrder.Price, sellOrder.Price);
            _conn.TransferShares(buyOrder.ClientID, sellOrder.ClientID, buyOrder.Ticker, amount, price);
            _conn.TransferBalance(buyOrder.ClientID, sellOrder.ClientID, (decimal)amount * price);
            // Console.WriteLine($"Client {buyOrder.ClientID} bought {amount} shares of {buyOrder.Ticker} from Client {sellOrder.ClientID} for ${price} each.");
            buyOrder.Amount -= amount;
            sellOrder.Amount -= amount;
        }

        private void ReleaseOrder(ref Order o)
        {
            if (o != null)
            {
                // Console.WriteLine($"ReleaseOrder ({o.Type})");
                if (o.Amount == 0)
                {
                    // Console.WriteLine($"******Filled '{o}'");
                    o.Fill();
                }
                Monitor.Exit(o);
            }
        }

        private void RemoveFilledOrder()
        {
            throw new NotImplementedException();
        }
    }

    class OrderBook
    {
        private Database _conn;
        private Dictionary<string, OrderQueue> _book;

        public OrderBook(Database conn)
        {
            _conn = conn; 
            _book = new Dictionary<string, OrderQueue>();
        }

        internal bool AddOrder(Order o)
        {
            switch (o.Type)
            {
                case OrderType.Buy:
                    {
                        if (!_conn.HasEnoughBalance(o.ClientID, o.Amount * o.Price))
                        {
                            Console.WriteLine($"Client {o.ClientID} does not have enough balance.");
                            return false;
                        }
                        else
                        {
                            GetQueue(o.Ticker).AddOrder(o);
                            return true;
                        }
                    }
                case OrderType.Sell:
                    {
                        if (!_conn.HasEnoughShares(o.ClientID, o.Ticker, o.Amount))
                        {
                            Console.WriteLine($"Client {o.ClientID} does not have enough shares.");
                            return false;
                        }
                        else
                        {
                            GetQueue(o.Ticker).AddOrder(o);
                            return true;
                        }
                    }
                default: return false;
            }
        }

        private OrderQueue GetQueue(string ticker)
        {
            if (!_book.ContainsKey(ticker))
            {
                MakeQueue(ticker);
            }
            return _book[ticker];
        }

        private void MakeQueue(string ticker)
        {
            _book.Add(ticker, new OrderQueue(_conn));
            _book[ticker].Start();
        }

        internal void Terminate()
        {
            foreach (OrderQueue q in _book.Values)
            {
                q.Terminate();
            }
        }
    }
}
