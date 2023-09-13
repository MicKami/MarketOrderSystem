using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketplaceOrderSystem
{
    public class Marketplace
    {
        Dictionary<string, SortedSet<MarketOrder>> _buyOrders;
        Dictionary<string, SortedSet<MarketOrder>> _sellOrders;

        public Marketplace()
        {

            _buyOrders = new Dictionary<string, SortedSet<MarketOrder>>();
            _sellOrders = new Dictionary<string, SortedSet<MarketOrder>>();
        }

        internal void PlaceOrder(MarketplaceOrderSystem.MarketOrder order, MarketPlayer player)
        {
            MarketOrder _order = new MarketOrder(order, player);
            var orderBook = _order.OrderType == MarketOrderType.Buy ? _buyOrders : _sellOrders;
            IComparer<MarketOrder> comparer = _order.OrderType == MarketOrderType.Buy ? new MarketOrder.BuyOrderComparer() : new MarketOrder.SellOrderComparer();
            if (!orderBook.ContainsKey(_order.ItemName))
            {
                orderBook.Add(_order.ItemName, new SortedSet<MarketOrder>(comparer));
            }
            orderBook[_order.ItemName].Add(_order);
            MatchOrder(_order);
        }
        private void MatchOrder(MarketOrder order)
        {
            var orderBook = order.OrderType == MarketOrderType.Buy ? _buyOrders : _sellOrders;
            var oppositeOrderBook = order.OrderType == MarketOrderType.Sell ? _buyOrders : _sellOrders;
            if (!oppositeOrderBook.ContainsKey(order.ItemName))
            {
                return;
            }

            var toRemove = new List<MarketOrder>();
            foreach (var oppositeOrder in oppositeOrderBook[order.ItemName])
            {
                if (oppositeOrder.Price <= order.Price)
                {
                    int tradeQuantity = Math.Min(order.Quantity, oppositeOrder.Quantity);
                    int transactionPrice = oppositeOrder.Price;
                    FillOrder(oppositeOrder, tradeQuantity);
                    order.Quantity -= tradeQuantity;
                    if (oppositeOrder.Quantity == 0)
                    {
                        toRemove.Add(oppositeOrder);
                    }
                    if (order.Quantity == 0)
                    {
                        orderBook[order.ItemName].Remove(order);
                        break;
                    }
                }
            }
            foreach (var filledOrder in toRemove)
            {
                oppositeOrderBook[order.ItemName].Remove(filledOrder);
            }
            if (oppositeOrderBook[order.ItemName].Count == 0) oppositeOrderBook.Remove(order.ItemName);
        }

        private void FillOrder(MarketOrder order, int tradeQuantity)
        {
            order.Quantity -= tradeQuantity;
            if(order.OrderType == MarketOrderType.Buy)
            {
                order.Owner.AddToInventory(order.ItemName, tradeQuantity);
            }
            else order.Owner.AddMoney(tradeQuantity * order.Price);
        }

        //        public void AddBuyOrder(MarketplaceOrderSystem.Market
        //Order buyOrder)
        //        {
        //            MarketOrder order = new MarketOrder(buyOrder);
        //            if (!_buyOrders.ContainsKey(order.ItemName))
        //            {
        //                _buyOrders.Add(order.ItemName, new SortedSet<MarketOrder>(new MarketOrder.BuyOrderComparer()));
        //            }
        //            _buyOrders[order.ItemName].Add(order);
        //            MatchBuyOrder(order);
        //        }

        //private void MatchBuyOrder(MarketOrder buyOrder)
        //{
        //    if (!_sellOrders.ContainsKey(buyOrder.ItemName))
        //    {
        //        return;
        //    }
        //    var toRemove = new List<MarketOrder>();
        //    foreach (var sellOrder in _sellOrders[buyOrder.ItemName])
        //    {
        //        if (sellOrder.Price <= buyOrder.Price)
        //        {
        //            int tradeQuantity = Math.Min(buyOrder.Quantity, sellOrder.Quantity);
        //            sellOrder.FillOrder(tradeQuantity);
        //            buyOrder.FillOrder(tradeQuantity);
        //            if (sellOrder.Quantity == 0)
        //            {
        //                toRemove.Add(sellOrder);
        //            }
        //            if (buyOrder.Quantity == 0)
        //            {
        //                _buyOrders[buyOrder.ItemName].Remove(buyOrder);
        //                break;
        //            }
        //        }
        //    }
        //    foreach (var filledOrder in toRemove)
        //    {
        //        _sellOrders[buyOrder.ItemName].Remove(filledOrder);
        //    }
        //    if (_sellOrders[buyOrder.ItemName].Count == 0) _sellOrders.Remove(buyOrder.ItemName);
        //}

        //public void AddSellOrder(MarketplaceOrderSystem.MarketOrder sellOrder)
        //{
        //    MarketOrder order = new MarketOrder(sellOrder);
        //    if (!_sellOrders.ContainsKey(order.ItemName))
        //    {
        //        _sellOrders.Add(order.ItemName, new SortedSet<MarketOrder>(new MarketOrder.SellOrderComparer()));
        //    }
        //    _sellOrders[order.ItemName].Add(order);
        //    MatchSellOrder(order);
        //}

        //private void MatchSellOrder(MarketOrder sellOrder)
        //{
        //    if (!_buyOrders.ContainsKey(sellOrder.ItemName))
        //    {
        //        return;
        //    }
        //    var toRemove = new List<MarketOrder>();
        //    foreach (var buyOrder in _buyOrders[sellOrder.ItemName])
        //    {
        //        if (sellOrder.Price <= buyOrder.Price)
        //        {
        //            int tradeQuantity = Math.Min(sellOrder.Quantity, buyOrder.Quantity);
        //            sellOrder.FillOrder(tradeQuantity);
        //            buyOrder.FillOrder(tradeQuantity);
        //            if (buyOrder.Quantity == 0)
        //            {
        //                toRemove.Add(buyOrder);
        //            }
        //            if (sellOrder.Quantity == 0)
        //            {
        //                _sellOrders[sellOrder.ItemName].Remove(sellOrder);
        //                break;
        //            }
        //        }
        //    }
        //    foreach (var filledOrder in toRemove)
        //    {
        //        _buyOrders[sellOrder.ItemName].Remove(filledOrder);
        //    }
        //    if (_buyOrders[sellOrder.ItemName].Count == 0) _buyOrders.Remove(sellOrder.ItemName);
        //}

        public List<MarketplaceOrderSystem.MarketOrder> GetOrders(string item, MarketOrderType orderType)
        {
            var orderBook = orderType == MarketOrderType.Buy ? _buyOrders : _sellOrders;
            List<MarketplaceOrderSystem.MarketOrder> orders = new List<MarketplaceOrderSystem.MarketOrder>();
            if (orderBook.ContainsKey(item))
            {
                foreach (var marketOrder in orderBook[item])
                {
                    orders.Add(new MarketplaceOrderSystem.MarketOrder(marketOrder.OrderType, marketOrder.ItemName, marketOrder.Quantity, marketOrder.Price));
                }
            }
            return orders;
        }
        public List<MarketplaceOrderSystem.MarketOrder> GetSellOrders(string item)
        {
            List<MarketplaceOrderSystem.MarketOrder> orders = new List<MarketplaceOrderSystem.MarketOrder>();
            if (_sellOrders.ContainsKey(item))
            {
                foreach (var marketOrder in _sellOrders[item])
                {
                    orders.Add(new MarketplaceOrderSystem.MarketOrder(marketOrder.OrderType, marketOrder.ItemName, marketOrder.Quantity, marketOrder.Price));
                }
            }
            return orders;
        }

        internal class MarketOrder
        {
            public MarketOrderType OrderType { get; }
            public string ItemName { get; }
            public int Quantity { get; internal set; }
            public int Price { get; }
            public MarketPlayer Owner { get; }
            public long Timestamp { get; }
            public MarketOrder(MarketOrderType orderType, string itemName, int quantity, int price, MarketPlayer player)
            {
                OrderType = orderType;
                ItemName = itemName;
                Quantity = quantity;
                Price = price;
                Owner = player;
                Timestamp = DateTime.Now.Ticks;
            }
            public MarketOrder(MarketplaceOrderSystem.MarketOrder order, MarketPlayer player) : this(order.OrderType, order.ItemName, order.Quantity, order.Price, player) { }
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
