using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Orderbook.DataAccessLayer;
using Orderbook.Entities;

namespace Orderbook.ServiceLayer
{
    class ClientInformation
    {
        private int _clientID;
        private Database _conn;

        public ClientInformation(int clientID, Database conn)
        {
            _clientID = clientID;
            _conn = conn;
        }

        internal Portfolio GetPortfolio()
        {
            return _conn.GetPortfolio(_clientID);
        }

        internal TransactionHistory GetTransactionHistory()
        {
            return _conn.GetTransactionHistory();
        }
    }
}
