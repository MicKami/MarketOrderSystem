using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketplaceOrderSystem
{
    public struct MarketOrder
    {
        public required MarketOrderType OrderType { get; init; }
        public required string ItemName { get; init; }
        public required int Quantity { get; init; }
        public required int Price { get; init; }
        [SetsRequiredMembers]
        public MarketOrder(MarketOrderType orderType, string itemName, int quantity, int price)
        {
            OrderType = orderType;
            ItemName = itemName;
            Quantity = quantity;
            Price = price;
        }
    }
}
