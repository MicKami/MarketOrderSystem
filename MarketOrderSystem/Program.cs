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
            //Marketplace market = new Marketplace();

            ////List<MarketOrder> sellOrders = new List<MarketOrder>()
            ////{
            ////    new MarketOrder("wood", 100, 26),
            ////    new MarketOrder("wood", 50, 25),
            ////    new MarketOrder("wood", 700, 27),
            ////    new MarketOrder("wood", 50, 23),
            ////};

            ////foreach (var order in sellOrders)
            ////{
            ////    market.AddSellOrder(order);
            ////}

            ////List<MarketOrder> buyOrders = new List<MarketOrder>()
            ////{
            ////    new MarketOrder("wood", 500, 26),
            ////};

            ////foreach (var order in buyOrders)
            ////{
            ////    market.AddBuyOrder(order);
            ////}

            //market.AddBuyOrder(new MarketOrder("wood", 100, 25));
            //market.AddBuyOrder(new MarketOrder("wood", 200, 25));
            //market.AddBuyOrder(new MarketOrder("wood", 300, 25));
            //market.AddBuyOrder(new MarketOrder("wood", 400, 25));
            //market.AddBuyOrder(new MarketOrder("wood", 500, 25));
            ////market.AddSellOrder(new MarketOrder("wood", 700, 24));

            //Console.WriteLine("Sell orders");
            //Console.WriteLine("| Item | Quantity | Price | Timestamp");
            //Console.WriteLine("-------------------------");
            //foreach (var order in market.GetSellOrders("wood"))
            //{
            //    Console.WriteLine($"| {order.ItemName} | {order.Quantity}      | {order.Price}    | {order.Timestamp}");
            //}

            //Console.WriteLine();

            //Console.WriteLine("Buy orders");
            //Console.WriteLine("| Item | Quantity | Price | Timestamp");
            //Console.WriteLine("-------------------------");
            //foreach (var order in market.GetBuyOrders("wood"))
            //{
            //    Console.WriteLine($"| {order.ItemName} | {order.Quantity}      | {order.Price}    | {order.Timestamp}");
            //}

            SortedSet<MarketOrder> orders = new SortedSet<MarketOrder>(new BuyOrderComparer())
            {
                new MarketOrder("wood", 102, 25),
                new MarketOrder("wood", 109, 22),
                new MarketOrder("wood", 103, 25),
                new MarketOrder("wood", 104, 25),
                new MarketOrder("wood", 101, 26),
                new MarketOrder("wood", 107, 23),
                new MarketOrder("wood", 105, 25),
                new MarketOrder("wood", 100, 27),
                new MarketOrder("wood", 108, 23),
                new MarketOrder("wood", 106, 25),
            };

            Console.Read();
        }
    }
}
