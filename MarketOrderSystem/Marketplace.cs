using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketOrderSystem
{
    public class Marketplace
    {
        private static readonly ReadOnlyCollection<MarketOrder> _empty = new List<MarketOrder>().AsReadOnly();
        Dictionary<string, SortedSet<MarketOrder>> _buyOrders;
        Dictionary<string, SortedSet<MarketOrder>> _sellOrders;

        public Marketplace()
        {

            _buyOrders = new Dictionary<string, SortedSet<MarketOrder>>();
            _sellOrders = new Dictionary<string, SortedSet<MarketOrder>>();
        }


        public void AddBuyOrder(MarketOrderSystem.MarketOrder buyOrder)
        {
            MarketOrder order = new MarketOrder(buyOrder);
            if (!_buyOrders.ContainsKey(order.ItemName))
            {
                _buyOrders.Add(order.ItemName, new SortedSet<MarketOrder>(new MarketOrder.BuyOrderComparer()));
            }
            _buyOrders[order.ItemName].Add(order);
            MatchBuyOrder(order);
        }

        private void MatchBuyOrder(MarketOrder buyOrder)
        {
            if (!_sellOrders.ContainsKey(buyOrder.ItemName))
            {
                return;
            }
            var toRemove = new List<MarketOrder>();
            foreach (var sellOrder in _sellOrders[buyOrder.ItemName])
            {
                if (sellOrder.Price <= buyOrder.Price)
                {
                    int tradeQuantity = Math.Min(buyOrder.Quantity, sellOrder.Quantity);
                    sellOrder.FillOrder(tradeQuantity);
                    buyOrder.FillOrder(tradeQuantity);
                    if (sellOrder.Quantity == 0)
                    {
                        toRemove.Add(sellOrder);
                    }
                    if (buyOrder.Quantity == 0)
                    {
                        _buyOrders[buyOrder.ItemName].Remove(buyOrder);
                        break;
                    }
                }
            }
            foreach (var filledOrder in toRemove)
            {
                _sellOrders[buyOrder.ItemName].Remove(filledOrder);
            }
        }

        public void AddSellOrder(MarketOrderSystem.MarketOrder sellOrder)   
        {
            MarketOrder order = new MarketOrder(sellOrder);
            if (!_sellOrders.ContainsKey(order.ItemName))
            {
                _sellOrders.Add(order.ItemName, new SortedSet<MarketOrder>(new MarketOrder.SellOrderComparer()));
            }
            _sellOrders[order.ItemName].Add(order);
            MatchSellOrder(order);
        }

        private void MatchSellOrder(MarketOrder sellOrder)
        {
            if (!_buyOrders.ContainsKey(sellOrder.ItemName))
            {
                return;
            }
            var toRemove = new List<MarketOrder>();
            foreach (var buyOrder in _buyOrders[sellOrder.ItemName])
            {
                if (sellOrder.Price <= buyOrder.Price)
                {
                    int tradeQuantity = Math.Min(sellOrder.Quantity, buyOrder.Quantity);
                    sellOrder.FillOrder(tradeQuantity);
                    buyOrder.FillOrder(tradeQuantity);
                    if (buyOrder.Quantity == 0)
                    {
                        toRemove.Add(buyOrder);
                    }
                    if (sellOrder.Quantity == 0)
                    {
                        _sellOrders[sellOrder.ItemName].Remove(sellOrder);
                        break;
                    }
                }
            }
            foreach (var filledOrder in toRemove)
            {
                _buyOrders[sellOrder.ItemName].Remove(filledOrder);
            }
        }

        private class MarketOrder
        {
            public MarketOrderType OrderType { get; }
            public string ItemName { get; }
            public int Quantity { get; private set; }
            public int Price { get; }
            public ulong Timestamp { get; }


            public MarketOrder(MarketOrderType orderType, string itemName, int quantity, int price)
            {
                OrderType = orderType;
                ItemName = itemName;
                Quantity = quantity;
                Price = price;
                Timestamp = (ulong)(DateTime.Now.Ticks - new DateTime(2000, 1, 1).Ticks);
            }
            public MarketOrder(MarketOrderSystem.MarketOrder order) : this(order.OrderType, order.ItemName, order.Quantity, order.Price) { }

            public void FillOrder(int quantity)
            {
                Quantity -= quantity;
            }
            public class SellOrderComparer : IComparer<MarketOrder>
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
                        return x.Price.CompareTo(y.Price);
                    }
                }
            }
            public class BuyOrderComparer : IComparer<MarketOrder>
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
    }
}
