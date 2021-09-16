using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orderbook.Entities
{
    class Portfolio
    {
        internal string ClientName { get; set; }
        internal Dictionary<string, int> Equities { get; set; }

        public Portfolio()
        {
            Equities = new Dictionary<string, int>();
        }
    }
}
