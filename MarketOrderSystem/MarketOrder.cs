using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketOrderSystem
{
    public readonly struct MarketOrder
    {
        public MarketOrderType OrderType { get; }
        public string ItemName { get; }
        public int Quantity { get; }
        public int Price { get; }
        public MarketOrder(MarketOrderType orderType, string itemName, int quantity, int price)
        {
            OrderType = orderType;
            ItemName = itemName;
            Quantity = quantity;
            Price = price;
        }
    }
}
