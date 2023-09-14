﻿using System;
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
        Dictionary<string, SortedSet<MarketOrder>> _buyOrders = new();
        Dictionary<string, SortedSet<MarketOrder>> _sellOrders = new();
        Dictionary<long, MarketOrder> _allOrders = new();
        public event Action<OrderFilledEventArgs>? OnOrderFilled;

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
            _allOrders.Add(_order.ID, _order);
            player.RegisterOrder(_order.ID, order);
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
                var buyOrder = order.OrderType == MarketOrderType.Buy ? order : oppositeOrder;
                var sellOrder = order.OrderType == MarketOrderType.Sell ? order : oppositeOrder;
                if (sellOrder.Price <= buyOrder.Price)
                {
                    int tradeQuantity = Math.Min(order.Quantity, oppositeOrder.Quantity);
                    int transactionPrice = oppositeOrder.Price;

                    FillSellOrder(sellOrder, tradeQuantity, transactionPrice);
                    FillBuyOrder(buyOrder, tradeQuantity, transactionPrice);
                    if (oppositeOrder.Quantity == 0)
                    {
                        toRemove.Add(oppositeOrder);
                    }
                    if (order.Quantity == 0)
                    {
                        orderBook[order.ItemName].Remove(order);
                        if (orderBook[order.ItemName].Count == 0)
                        {
                            orderBook.Remove(order.ItemName);
                            _allOrders.Remove(order.ID);
                        }
                        break;
                    }
                }
            }
            foreach (var filledOrder in toRemove)
            {
                oppositeOrderBook[order.ItemName].Remove(filledOrder);
                _allOrders.Remove(filledOrder.ID);
            }
            if (oppositeOrderBook[order.ItemName].Count == 0)
            {
                oppositeOrderBook.Remove(order.ItemName);
            }
        }

        internal void CancelOrder(long orderID)
        {
            var order = _allOrders[orderID];
            if(order.OrderType == MarketOrderType.Buy)
            {
                order.Owner.AddMoney(order.Price * order.Quantity);
                order.Owner.FillOrder(orderID);
                _allOrders.Remove(orderID);
                _buyOrders[order.ItemName].Remove(order);
                if (_buyOrders[order.ItemName].Count == 0) _buyOrders.Remove(order.ItemName);
            }
            else
            {
                order.Owner.AddToInventory(order.ItemName, order.Quantity);
                order.Owner.FillOrder(orderID);
                _allOrders.Remove(orderID);
                _sellOrders[order.ItemName].Remove(order);
                if (_sellOrders[order.ItemName].Count == 0) _sellOrders.Remove(order.ItemName);
            }
        }

        private void FillBuyOrder(MarketOrder buyOrder, int tradeQuantity, int transactionPrice)
        {
            buyOrder.Quantity -= tradeQuantity;
            buyOrder.Owner.AddToInventory(buyOrder.ItemName, tradeQuantity);
            buyOrder.Owner.AddMoney((buyOrder.Price - transactionPrice) * tradeQuantity);
            buyOrder.Owner.FillOrder(buyOrder.ID);
        }

        private void FillSellOrder(MarketOrder sellOrder, int tradeQuantity, int transactionPrice)
        {
            sellOrder.Quantity -= tradeQuantity;
            sellOrder.Owner.AddMoney(transactionPrice * tradeQuantity);
            sellOrder.Owner.FillOrder(sellOrder.ID);
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

        private class MarketOrder
        {
            public MarketOrderType OrderType { get; }
            public string ItemName { get; }
            public int Quantity { get; internal set; }
            public int Price { get; }
            public MarketPlayer Owner { get; }
            public long Timestamp { get; }
            public long ID { get; }

            public MarketOrder(MarketOrderType orderType, string itemName, int quantity, int price, MarketPlayer player)
            {
                OrderType = orderType;
                ItemName = itemName;
                Quantity = quantity;
                Price = price;
                Owner = player;
                Timestamp = DateTime.UtcNow.Ticks;
                ID = Generate.ID();
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
