using Kaos.Collections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketplaceOrderSystem
{
    public class Marketplace
    {
        Dictionary<string, SortedList<int, List<MarketOrder>>> _buyOrders;
        Dictionary<string, SortedList<int, List<MarketOrder>>> _sellOrders;

        public Marketplace()
        {
            
            _buyOrders = new Dictionary<string, SortedList<int, List<MarketOrder>>>();
            _sellOrders = new Dictionary<string, SortedList<int, List<MarketOrder>>>();
            
        }


        public void AddBuyOrder(MarketOrder order)
        {
            if (!_buyOrders.ContainsKey(order.ItemName))
            {
                _buyOrders.Add(order.ItemName, new SortedList<int, List<MarketOrder>>(Comparer<int>.Create((x, y) => y.CompareTo(x))));
            }
            if (!_buyOrders[order.ItemName].ContainsKey(order.Price))
            {
                _buyOrders[order.ItemName].Add(order.Price, new List<MarketOrder>());
            }
            _buyOrders[order.ItemName][order.Price].Add(order);

            MatchBuyOrder(order);
        }

        private void MatchBuyOrder(MarketOrder buyOrder)
        {
            if (!_sellOrders.ContainsKey(buyOrder.ItemName))
            {
                return;
            }

            var prices = _sellOrders[buyOrder.ItemName].Keys;
            for (int i = 0; i < prices.Count; i++)
            {
                if (prices[i] <= buyOrder.Price)
                {
                    int count = _sellOrders[buyOrder.ItemName][prices[i]].Count;
                    for (int j = 0; j < count; j++)
                    {
                        MarketOrder sellOrder = _sellOrders[buyOrder.ItemName][prices[i]][j];
                        if (buyOrder.Quantity > 0)
                        {
                            int tradeQuantity = Math.Min(buyOrder.Quantity, sellOrder.Quantity);
                            buyOrder.Quantity -= tradeQuantity;
                            sellOrder.Quantity -= tradeQuantity;

                            if (sellOrder.Quantity == 0)
                            {
                                _sellOrders[buyOrder.ItemName][prices[i]].Remove(sellOrder);
                            }
                            if (buyOrder.Quantity == 0)
                            {
                                _buyOrders[buyOrder.ItemName][buyOrder.Price].Remove(buyOrder);
                            }
                        }
                        else return;
                    }
                }
                else break;
            }
        }

        public void AddSellOrder(MarketOrder order)
        {
            if (!_sellOrders.ContainsKey(order.ItemName))
            {
                _sellOrders.Add(order.ItemName, new SortedList<int, List<MarketOrder>>(Comparer<int>.Create((x, y) => x.CompareTo(y))));
            }
            if (!_sellOrders[order.ItemName].ContainsKey(order.Price))
            {
                _sellOrders[order.ItemName].Add(order.Price, new List<MarketOrder>());
            }
            _sellOrders[order.ItemName][order.Price].Add(order);
            MatchSellOrder(order);
        }

        private void MatchSellOrder(MarketOrder sellOrder)
        {
            if (!_buyOrders.ContainsKey(sellOrder.ItemName))
            {
                return;
            }

            var prices = _buyOrders[sellOrder.ItemName].Keys;
            for (int i = 0; i < prices.Count; i++)
            {
                if (prices[i] >= sellOrder.Price)
                {
                    int count = _buyOrders[sellOrder.ItemName][prices[i]].Count;
                    for (int j = 0; j < count; j++)
                    {
                        MarketOrder buyOrder = _buyOrders[sellOrder.ItemName][prices[i]][j];
                        if (sellOrder.Quantity > 0)
                        {
                            int tradeQuantity = Math.Min(sellOrder.Quantity, buyOrder.Quantity);
                            sellOrder.Quantity -= tradeQuantity;
                            buyOrder.Quantity -= tradeQuantity;

                            if (buyOrder.Quantity == 0)
                            {
                                _buyOrders[sellOrder.ItemName][prices[i]].Remove(buyOrder);
                            }
                            if (sellOrder.Quantity == 0)
                            {
                                _sellOrders[sellOrder.ItemName][sellOrder.Price].Remove(sellOrder);
                            }
                        }
                        else return;
                    }
                }
                else break;
            }
        }

        public IReadOnlyCollection<MarketOrder> GetBuyOrders(string item)
        {
            List<MarketOrder> results = new List<MarketOrder>();
            if (_buyOrders.ContainsKey(item))
            {
                foreach (List<MarketOrder> orders in _buyOrders[item].Values)
                {
                    results.AddRange(orders);
                }
            }
            return results;
        }
        public IReadOnlyCollection<MarketOrder> GetSellOrders(string item)
        {
            List<MarketOrder> results = new List<MarketOrder>();
            if (_sellOrders.ContainsKey(item))
            {
                foreach (List<MarketOrder> orders in _sellOrders[item].Values)
                {
                    results.AddRange(orders);
                }
            }
            return results;
        }
    }
}
