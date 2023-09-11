using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketOrderSystem
{
    internal class BuyOrderComparer : IComparer<MarketOrder>
    {
        public int Compare(MarketOrder? x, MarketOrder? y)
        {
            if (x == null || y == null) return 0;
            if (x.Price.CompareTo(y.Price) == 0)
            {
                return x.Timestamp.CompareTo(y.Timestamp);
            }
            else
            {
                return y.Price.CompareTo(x.Price);
            }
        }
    }
}
