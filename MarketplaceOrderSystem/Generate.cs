using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketplaceOrderSystem
{
    internal static class Generate
    {
        private static long _nextID = DateTime.UtcNow.Ticks;
        public static long ID()
        {
            return Interlocked.Increment(ref _nextID);
        }
    }
}
