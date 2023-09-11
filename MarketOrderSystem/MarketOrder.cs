using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketOrderSystem
{
    public class MarketOrder
    {
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public int Price { get; set; }
        public readonly ulong Timestamp;

        public MarketOrder(string itemName, int quantity, int price)
        {
            ItemName = itemName;
            Quantity = quantity;
            Price = price;
            Timestamp = (ulong)(DateTime.Now.Ticks - new DateTime(2000, 1, 1).Ticks);
        }
    }
}
