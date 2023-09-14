namespace MarketplaceOrderSystem
{
    public class MarketPlayer
    {
        private Dictionary<long, MarketOrder> _orders = new Dictionary<long, MarketOrder>();
        private Dictionary<string, int> _inventory = new Dictionary<string, int>();
        private int _currency;
        public string Name { get; init; }
        public MarketPlayer(string name)
        {
            Name = name;
        }
        public void AddToInventory(string item, int quantity)
        {
            if (quantity > 0)
            {
                if (!_inventory.TryAdd(item, quantity))
                {
                    _inventory[item] += quantity;
                }
            }
        }
        public bool RemoveFromInventory(string item, int quantity)
        {
            if (quantity > 0)
            {
                if (_inventory.ContainsKey(item))
                {
                    if (_inventory[item] >= quantity)
                    {
                        _inventory[item] -= quantity;
                        if (_inventory[item] == 0)
                        {
                            _inventory.Remove(item);
                        }
                        return true;
                    }
                }
            }
            return false;
        }
        public int GetMoney()
        {
            return _currency;
        }
        public void AddMoney(int amount)
        {
            if (amount > 0)
            {
                _currency += amount;
            }
        }
        public bool SubtractMoney(int amount)
        {
            if (amount > 0)
            {
                if (_currency >= amount)
                {
                    _currency -= amount;
                    return true;
                }
            }
            return false;
        }
        public List<(string itemName, int quantity)> GetInventory()
        {
            List<(string itemName, int quantity)> inventory = new List<(string itemName, int quantity)>();
            foreach (var kvp in _inventory)
            {
                inventory.Add((kvp.Key, kvp.Value));
            }
            return inventory;
        }
        public List<MarketOrder> GetOrders()
        {
            return _orders.Values.ToList();
        }
        public bool IsOrderValid(MarketOrder order)
        {
            if (order.Quantity < 0 || order.Price < 0)
            {
                return false;
            }
            if (order.OrderType == MarketOrderType.Sell)
            {
                return (_inventory.ContainsKey(order.ItemName) && _inventory[order.ItemName] >= order.Quantity);
            }
            else return order.Quantity * order.Price <= _currency;
        }
        public bool PlaceOrder(MarketOrder order, Marketplace market)
        {
            if (ProcessOrder(order))
            {
                market.PlaceOrder(order, this);
                return true;
            }
            return false;
        }

        public bool CancelOrder(int enumerationID, Marketplace market)
        {
            if(enumerationID <= _orders.Count) 
            {
                market.CancelOrder(_orders.Keys.ToArray()[enumerationID]);
                return true;
            }
            return false;
        }

        private bool ProcessOrder(MarketOrder order)
        {
            if (order.OrderType == MarketOrderType.Sell)
            {
                if (RemoveFromInventory(order.ItemName, order.Quantity))
                {
                    return true;
                }
            }
            else
            {
                if (SubtractMoney(order.Quantity * order.Price))
                {
                    return true;
                }
            }
            return false;
        }
        internal void RegisterOrder(long orderID, MarketOrder order)
        {
            _orders[orderID] = order;
        }
        internal void FillOrder(long orderID)
        {
            _orders.Remove(orderID);
        }

    }
}
