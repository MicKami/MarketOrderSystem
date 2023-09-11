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


        public void AddBuyOrder(MarketOrder order)
        {
            if (!_buyOrders.ContainsKey(order.ItemName))
            {
                _buyOrders.Add(order.ItemName, new SortedSet<MarketOrder>(new BuyOrderComparer()));
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
                if (buyOrder.Price >= sellOrder.Price)
                {
                    int tradeQuantity = Math.Min(buyOrder.Quantity, sellOrder.Quantity);
                    buyOrder.Quantity -= tradeQuantity;
                    sellOrder.Quantity -= tradeQuantity;
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

        public void AddSellOrder(MarketOrder order)
        {
            if (!_sellOrders.ContainsKey(order.ItemName))
            {
                _sellOrders.Add(order.ItemName, new SortedSet<MarketOrder>(new SellOrderComparer()));
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
                    sellOrder.Quantity -= tradeQuantity;
                    buyOrder.Quantity -= tradeQuantity;
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

        public IReadOnlyCollection<MarketOrder> GetBuyOrders(string item)
        {
            if (_buyOrders.ContainsKey(item))
            {
                return _buyOrders[item];

            }
            else return _empty;
        }
        public IReadOnlyCollection<MarketOrder> GetSellOrders(string item)
        {
            if (_sellOrders.ContainsKey(item))
            {
                return _sellOrders[item];

            }
            else return _empty;
        }
    }
}
