using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orderbook.Entities
{
    class Transaction
    {
        internal DateTime TransactionTime { get; set; }
        internal string BuyerName { get; set; }
        internal string SellerName { get; set; }
        internal string Ticker { get; set; }
        internal int Amount { get; set; }
        internal decimal Price { get; set; }

        public override string ToString()
        {
            return $"At {TransactionTime}, {BuyerName} bought {Amount} shares of {Ticker} from {SellerName} for ${Price} each.";
        }
    }

    class TransactionHistory
    {
        internal List<Transaction> TransactionBook;

        public TransactionHistory()
        {
            TransactionBook = new List<Transaction>();
        }

        internal void Add(Transaction t)
        {
            TransactionBook.Add(t);
        }
    }
}
