namespace MarketplaceOrderSystem
{
    public class Marketplace
    {
        Dictionary<string, SortedSet<MarketOrder>> _buyOrders = new();
        Dictionary<string, SortedSet<MarketOrder>> _sellOrders = new();
        Dictionary<long, MarketOrder> _allOrders = new();
        public event Action<MarketTranscationEventArgs>? OnTranscactionComplete;

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
                    OnTranscactionComplete?.Invoke(new MarketTranscationEventArgs(buyOrder.Owner, sellOrder.Owner, order.ItemName, tradeQuantity, transactionPrice));
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
            if (order.OrderType == MarketOrderType.Buy)
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


        private class MarketOrder
        {
            public MarketOrderType OrderType { get; }
            public string ItemName { get; }
            public int Quantity { get; internal set; }
            public int Price { get; }
            public MarketPlayer Owner { get; }
            public long Timestamp { get; }
            public long ID { get; }

            public MarketOrder(MarketplaceOrderSystem.MarketOrder order, MarketPlayer player)
            {
                OrderType = order.OrderType;
                ItemName = order.ItemName;
                Quantity = order.Quantity;
                Price = order.Price;
                Owner = player;
                Timestamp = DateTime.UtcNow.Ticks;
                ID = Generate.ID();
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
