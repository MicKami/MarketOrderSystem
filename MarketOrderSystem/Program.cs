using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using MarketplaceOrderSystem;

class Program
{
    static void Main(string[] args)
    {
        Marketplace market = new Marketplace();


        MarketPlayer player1 = new MarketPlayer("Player1");
        player1.AddToInventory("wood", 100);



        MarketPlayer player2 = new MarketPlayer("Player2");
        player2.AddMoney(10000);

        player2.PlaceOrder(new MarketOrder { ItemName = "wood", OrderType = MarketOrderType.Buy, Price = 25, Quantity = 100 }, market);
        player1.PlaceOrder(new MarketOrder { ItemName = "wood", OrderType = MarketOrderType.Sell, Price = 20, Quantity = 100 }, market);

        DisplayPlayerInventory(player1);
        DisplayPlayerOrders(player1);
        DisplayPlayerInventory(player2);
        DisplayPlayerOrders(player2);
        DisplayMarketOrders("wood", market);

        Console.Read();

        static void DisplayPlayerInventory(MarketPlayer player)
        {
            Console.WriteLine($"---------{player.Name} Inventory --------- ");
            Console.WriteLine($"Money: {player.GetMoney()}");
            foreach (var inventoryItem in player.GetInventory())
            {
                Console.WriteLine($"{inventoryItem.itemName}: {inventoryItem.quantity}");
            }
            Console.WriteLine("-------------------------------\n");
        }

        static void DisplayPlayerOrders(MarketPlayer player)
        {
            Console.WriteLine($"---------{player.Name} Orders ---------");
            int i = 0;
            foreach (var order in player.GetOrders())
            {
                Console.WriteLine($"{i++}. {{ {order.OrderType}| {order.ItemName} | {order.Quantity} | {order.Price} }}");
            }
            Console.WriteLine("-------------------------------\n");
        }

        static void DisplayMarketOrders(string item, Marketplace market)
        {
            Console.WriteLine($"--------- {item} Market --------- ");
            Console.WriteLine($"Buy");
            foreach (var buyOrder in market.GetOrders(item, MarketOrderType.Buy).Reverse<MarketOrder>())
            {
                Console.WriteLine($"| {buyOrder.ItemName}| {buyOrder.Quantity} | {buyOrder.Price} |");
            }
            Console.WriteLine("+---------------+");
            foreach (var sellOrder in market.GetOrders(item, MarketOrderType.Sell))
            {
                Console.WriteLine($"| {sellOrder.ItemName}| {sellOrder.Quantity} | {sellOrder.Price} |");
            }
            Console.WriteLine($"Sell");
            Console.WriteLine("-------------------------------\n");
        }
    }

}

