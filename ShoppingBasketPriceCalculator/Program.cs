using System;
using System.Collections.Generic;
using System.Linq;

namespace ShoppingBasket
{
    public class Item
    {
        public string Name { get; }
        public decimal Price { get; }
        public SpecialOffer? Offer { get; }

        public Item(string name, decimal price, SpecialOffer? offer = null)
        {
            Name = name;
            Price = price;
            Offer = offer;
        }
    }

    public class SpecialOffer
    {
        public int Quantity { get; }
        public int PriceForQuantity { get; }

        public SpecialOffer(int quantity, int priceForQuantity)
        {
            Quantity = quantity;
            PriceForQuantity = priceForQuantity;
        }
    }

    public class Basket
    {
        private readonly Dictionary<string, Item> _catalog;

        public Basket()
        {
            _catalog = new Dictionary<string, Item>
            {
                { "Apple", new Item("Apple", 0.35m) },
                { "Banana", new Item("Banana", 0.20m) },
                { "Melon", new Item("Melon", 0.50m, new SpecialOffer(2, 1)) },
                { "Lime", new Item("Lime", 0.15m, new SpecialOffer(3, 2)) }
            };
        }

        public decimal CalculateTotal(List<string> items)
        {
            var groupedItems = items
                .GroupBy(x => x)
                .ToDictionary(g => g.Key, g => g.Count());

            decimal total = 0;

            foreach (var group in groupedItems)
            {
                if (!_catalog.TryGetValue(group.Key, out var item))
                {
                    throw new ArgumentException($"Unknown item: {group.Key}");
                }

                if (item.Offer == null)
                {
                    total += item.Price * group.Value;
                }
                else
                {
                    var specialOffer = item.Offer;
                    int quantity = group.Value;

                    // Calculate number of complete offer sets (e.g., pairs for melons, triplets for limes)
                    int completeSets = quantity / specialOffer.Quantity;

                    // Calculate remaining items not part of a complete set
                    int remainingItems = quantity % specialOffer.Quantity;

                    // Calculate total price:
                    // 1. Complete sets at offer price
                    // 2. Remaining items at full price
                    decimal offerSetsCost = completeSets * item.Price * specialOffer.PriceForQuantity;
                    decimal remainingCost = remainingItems * item.Price;

                    total += offerSetsCost + remainingCost;
                }
            }

            return total;
        }
    }

    public class Program
    {
        public static void Main()
        {
            var basket = new Basket();

            // Example usage with different scenarios
            var shoppingList = new List<string>
            {
                "Apple", "Apple",           // 2 × 35p = 70p
                "Banana",                   // 1 × 20p = 20p
                "Melon", "Melon", "Melon",  // (1 × 50p) + (1 free) + (1 × 50p) = Rs1.00
                "Lime", "Lime", "Lime",     // (2 × 15p) + (1 free) = 30p
                "Lime"                      // 1 × 15p = 15p
            };

            decimal total = basket.CalculateTotal(shoppingList);
            Console.WriteLine($"Total cost: Rs{total:F2}");

            // Test with just melons
            var melonTest = new List<string> { "Melon", "Melon", "Melon" };
            decimal melonTotal = basket.CalculateTotal(melonTest);
            Console.WriteLine($"Melon test (3 melons) cost: Rs{melonTotal:F2}"); // Should be Rs1.00

            // Test with just limes
            var limeTest = new List<string> { "Lime", "Lime", "Lime", "Lime" };
            decimal limeTotal = basket.CalculateTotal(limeTest);
            Console.WriteLine($"Lime test (4 limes) cost: Rs{limeTotal:F2}"); // Should be Rs0.45
        }
    }
}