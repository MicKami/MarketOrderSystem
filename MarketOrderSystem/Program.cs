using MarketplaceOrderSystem;

class Program
{
    static void Main(string[] args)
    {
        Marketplace market = new Marketplace();
        market.OnTranscactionComplete += Market_OnOrderFilled;


        MarketPlayer player1 = new MarketPlayer("Player1");
        player1.AddToInventory("wood", 100);

        MarketPlayer player2 = new MarketPlayer("Player2");
        player2.AddMoney(10000);

        player1.PlaceOrder(new MarketOrder { ItemName = "wood", OrderType = MarketOrderType.Sell, Price = 20, Quantity = 100 }, market);
        player2.PlaceOrder(new MarketOrder { ItemName = "wood", OrderType = MarketOrderType.Buy, Price = 25, Quantity = 100 }, market);




        Console.Read();

    }
    private static void DisplayPlayerInventory(MarketPlayer player)
    {
        Console.WriteLine($"---------{player.Name} Inventory --------- ");
        Console.WriteLine($"Money: {player.GetMoney()}");
        foreach (var inventoryItem in player.GetInventory())
        {
            Console.WriteLine($"{inventoryItem.itemName}: {inventoryItem.quantity}");
        }
        Console.WriteLine("-------------------------------\n");
    }
    private static void DisplayPlayerOrders(MarketPlayer player)
    {
        Console.WriteLine($"---------{player.Name} Orders ---------");
        int i = 0;
        foreach (var order in player.GetOrders())
        {
            Console.WriteLine($"{i++}. {{ {order.OrderType}| {order.ItemName} | {order.Quantity} | {order.Price} }}");
        }
        Console.WriteLine("-------------------------------\n");
    }
    private static void DisplayMarketOrders(string item, Marketplace market)
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
    private static void Market_OnOrderFilled(MarketTranscationEventArgs args)
    {
        Console.WriteLine($"{args.Buyer.Name} bought x{args.Quantity} {args.Item} at a price of {args.Price} from {args.Seller.Name}");
    }
}

