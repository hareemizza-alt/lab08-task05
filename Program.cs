using System;
using System.Collections.Generic;

class InvalidBidException : Exception
{
    public InvalidBidException(string message) : base(message)
    {
    }
}

class BidEventArgs : EventArgs
{
    public string ItemName { get; set; }
    public string BidderName { get; set; }
    public double Amount { get; set; }
}

class Auction
{
    public string ItemName { get; set; }
    public double HighestBid { get; private set; }
    public event EventHandler<BidEventArgs> BidPlaced;
    public Auction(string itemName)
    {
        ItemName = itemName;
        HighestBid = 0;
    }
    protected virtual void OnBidPlaced(string bidderName, double amount)
    {
        BidPlaced?.Invoke(
            this,
            new BidEventArgs
            {
                ItemName = ItemName,
                BidderName = bidderName,
                Amount = amount
            });
    }
    public void PlaceBid(string bidderName, double amount)
    {
        if (amount <= HighestBid)
        {
            throw new InvalidBidException("Bid must be higher than the current highest bid.");
        }
        HighestBid = amount;
        OnBidPlaced(bidderName, amount);
    }
}

class ScreenDisplay
{
    public void ShowBid(object sender, BidEventArgs e)
    {
        Console.WriteLine("New highest bid: " + e.BidderName + " bid " + e.Amount + " on " + e.ItemName);
    }
}
class SellerNotifier
{
    public List<string> Messages { get; } = new List<string>();
    public void NotifySeller(object sender, BidEventArgs e)
    {
        string message = "Seller notification: " +e.BidderName + " placed a bid of " + e.Amount + " on " + e.ItemName;
        Messages.Add(message);
        Console.WriteLine(message);
    }
}
class Program
{
    static void Main()
    {
        Auction auction = new Auction("Gaming Laptop");
        ScreenDisplay screen = new ScreenDisplay();
        SellerNotifier seller = new SellerNotifier();
        auction.BidPlaced += screen.ShowBid;
        auction.BidPlaced += seller.NotifySeller;
        Console.WriteLine("AUCTION STARTED");
        try
        {
            auction.PlaceBid("Izza", 50000);
            auction.PlaceBid("Faha", 60000);
            auction.PlaceBid("Ifra", 55000);
        }
        catch (InvalidBidException ex)
        {
            Console.WriteLine("Invalid bid: " + ex.Message);
        }
        auction.BidPlaced -= screen.ShowBid;
        Console.WriteLine("\nScreenDisplay unsubscribed.");
        try
        {
            auction.PlaceBid("Ifra", 70000);
        }
        catch (InvalidBidException ex)
        {
            Console.WriteLine("Invalid bid: " + ex.Message);
        }
        Console.WriteLine("\nSELLER MESSAGES ");
        foreach (string message in seller.Messages)
        {
            Console.WriteLine(message);
        }
    }
}