namespace MarketplaceOrderSystem
{
    public class MarketTranscationEventArgs : EventArgs
    {
        public MarketPlayer Buyer { get; init; }
        public MarketPlayer Seller { get; init; }
        public string Item { get; init; }
        public int Quantity { get; init; }
        public int Price { get; init; }
        public MarketTranscationEventArgs(MarketPlayer buyer, MarketPlayer seller, string item, int quantity, int price)
        {
            Buyer = buyer;
            Seller = seller;
            Item = item;
            Quantity = quantity;
            Price = price;
        }
    }
}