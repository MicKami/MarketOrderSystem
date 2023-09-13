using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarketplaceOrderSystem;

class Program
{
    static void Main(string[] args)
    {
        Marketplace market = new Marketplace();

        MarketPlayer player = new MarketPlayer("Player1");
        player.AddMoney(10000);
        player.AddToInventory("wood", 200);
        player.AddToInventory("stone", 300);
        player.AddToInventory("leather", 80);
        player.AddToInventory("iron", 50);
        MarketOrder order1 = new()
        {
            ItemName = "wood",
            OrderType = MarketOrderType.Sell,
            Price = 26,
            Quantity = 100
        };
        MarketOrder order2 = new()
        {
            ItemName = "stone",
            OrderType = MarketOrderType.Buy,
            Price = 20,
            Quantity = 50
        };
        player.PlaceOrder(order1, market);
        player.PlaceOrder(order2, market);

        DisplayPlayerInventory(player);
        DisplayOrders("wood", MarketOrderType.Sell, market);
        DisplayOrders("stone", MarketOrderType.Buy, market);

        Console.Read();

        static void DisplayPlayerInventory(MarketPlayer player)
        {
            //Console.WriteLine("-------------------------------");
            Console.WriteLine($"{player.Name}: ");
            Console.WriteLine($"Money: {player.GetMoney()}");
            foreach (var inventoryItem in player.GetInventory())
            {
                Console.WriteLine($"{inventoryItem.itemName}: {inventoryItem.quantity}");
            }
            Console.WriteLine("-------------------------------");
        }
    }

    private static void DisplayOrders(string item, MarketOrderType orderType, Marketplace market)
    {
        Console.WriteLine($"{orderType}\n");
        foreach (var buyOrder in market.GetOrders(item, orderType))
        {
            Console.WriteLine($"| {buyOrder.ItemName}| {buyOrder.Quantity} | {buyOrder.Price} |");
        }
        Console.WriteLine("\n-------------------------------");
    }
}

