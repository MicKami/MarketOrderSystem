using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketOrderSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            Marketplace market = new Marketplace();

            market.AddBuyOrder(new MarketOrder("wood", 900, 19));
            market.AddBuyOrder(new MarketOrder("wood", 700, 21));
            market.AddBuyOrder(new MarketOrder("wood", 800, 21));
            market.AddBuyOrder(new MarketOrder("wood", 300, 22));

            market.AddSellOrder(new MarketOrder("wood", 200, 23));
            market.AddSellOrder(new MarketOrder("wood", 700, 24));
            market.AddSellOrder(new MarketOrder("wood", 800, 25));
            market.AddSellOrder(new MarketOrder("wood", 500, 26));
            market.AddSellOrder(new MarketOrder("wood", 100, 28));

            foreach (var order in market.GetBuyOrders("wood").Reverse())
            {
                Console.WriteLine($"| {order.ItemName} | {order.Quantity} | {order.Price} |");
            }
            Console.WriteLine("+------+-----+----+");
            foreach (var order in market.GetSellOrders("wood"))
            {
                Console.WriteLine($"| {order.ItemName} | {order.Quantity} | {order.Price} |");
            }

            Console.Read();
        }
    }
}
