using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace ShoppingBasket.Tests
{
    [TestFixture]
    public class BasketTests
    {
        private Basket _basket;

        [SetUp]
        public void Setup()
        {
            _basket = new Basket();
        }

        [Test]
        public void EmptyBasket_ReturnsZero()
        {
            // Arrange
            var items = new List<string>();

            // Act
            var total = _basket.CalculateTotal(items);

            // Assert
            Assert.That(total, Is.EqualTo(0m));
        }

        [Test]
        public void UnknownItem_ThrowsArgumentException()
        {
            // Arrange
            var items = new List<string> { "InvalidItem" };

            // Act & Assert
            Assert.That(() => _basket.CalculateTotal(items),
                Throws.ArgumentException);
        }

        [TestCase(1, 0.35)] // Single apple
        [TestCase(2, 0.70)] // Two apples
        [TestCase(3, 1.05)] // Three apples
        public void Apples_CalculatesCorrectTotal(int count, decimal expectedTotal)
        {
            // Arrange
            var items = new List<string>();
            for (int i = 0; i < count; i++)
                items.Add("Apple");

            // Act
            var total = _basket.CalculateTotal(items);

            // Assert
            Assert.That(total, Is.EqualTo(expectedTotal));
        }

        [TestCase(1, 0.20)] // Single banana
        [TestCase(2, 0.40)] // Two bananas
        [TestCase(3, 0.60)] // Three bananas
        public void Bananas_CalculatesCorrectTotal(int count, decimal expectedTotal)
        {
            // Arrange
            var items = new List<string>();
            for (int i = 0; i < count; i++)
                items.Add("Banana");

            // Act
            var total = _basket.CalculateTotal(items);

            // Assert
            Assert.That(total, Is.EqualTo(expectedTotal));
        }

        [TestCase(1, 0.50)]  // One melon
        [TestCase(2, 0.50)]  // Two melons (buy one get one free)
        [TestCase(3, 1.00)]  // Three melons (one offer pair + one full price)
        [TestCase(4, 1.00)]  // Four melons (two offer pairs)
        [TestCase(5, 1.50)]  // Five melons (two offer pairs + one full price)
        [TestCase(6, 1.50)]  // Six melons (three offer pairs)
        public void Melons_WithBuyOneGetOneFree_CalculatesCorrectTotal(int count, decimal expectedTotal)
        {
            // Arrange
            var items = new List<string>();
            for (int i = 0; i < count; i++)
                items.Add("Melon");

            // Act
            var total = _basket.CalculateTotal(items);

            // Assert
            Assert.That(total, Is.EqualTo(expectedTotal));
        }

        [TestCase(1, 0.15)]  // One lime
        [TestCase(2, 0.30)]  // Two limes
        [TestCase(3, 0.30)]  // Three limes (three for price of two)
        [TestCase(4, 0.45)]  // Four limes (one offer group + one full price)
        [TestCase(5, 0.60)]  // Five limes (one offer group + two full price)
        [TestCase(6, 0.60)]  // Six limes (two offer groups)
        [TestCase(7, 0.75)]  // Seven limes (two offer groups + one full price)
        public void Limes_WithThreeForTwo_CalculatesCorrectTotal(int count, decimal expectedTotal)
        {
            // Arrange
            var items = new List<string>();
            for (int i = 0; i < count; i++)
                items.Add("Lime");

            // Act
            var total = _basket.CalculateTotal(items);

            // Assert
            Assert.That(total, Is.EqualTo(expectedTotal));
        }

        [Test]
        public void MixedBasket_CalculatesCorrectTotal()
        {
            // Arrange
            var items = new List<string>
            {
                "Apple",    // 0.35
                "Apple",    // 0.35
                "Banana",   // 0.20
                "Melon",    // 0.50
                "Melon",    // free
                "Melon",    // 0.50
                "Lime",     // 0.15
                "Lime",     // 0.15
                "Lime",     // free
                "Lime"      // 0.15
            };

            // Expected total:
            // 2 Apples: 0.70
            // 1 Banana: 0.20
            // 3 Melons: 1.00
            // 4 Limes: 0.45
            // Total: 2.35

            // Act
            var total = _basket.CalculateTotal(items);

            // Assert
            Assert.That(total, Is.EqualTo(2.35m));
        }

        [Test]
        public void LargeQuantities_CalculatesCorrectTotal()
        {
            // Arrange
            var items = new List<string>();
            // Add 100 of each item
            for (int i = 0; i < 100; i++)
            {
                items.Add("Apple");
                items.Add("Banana");
                items.Add("Melon");
                items.Add("Lime");
            }

            // Expected total:
            // 100 Apples: 100 × 0.35 = 35.00
            // 100 Bananas: 100 × 0.20 = 20.00
            // 100 Melons: 50 × 0.50 = 25.00 (50 pairs)
            // 100 Limes: 66 × 0.15 + 0.15 = 9.90 + 0.15 (33 sets of 3, plus 1 full price)
            decimal expectedTotal = 90.05m;

            // Act
            var total = _basket.CalculateTotal(items);

            // Assert
            Assert.That(total, Is.EqualTo(expectedTotal));
        }

        [Test]
        public void MultipleOffersInSameBasket_CalculatesCorrectTotal()
        {
            // Arrange
            var items = new List<string>
            {
                // Melons with BOGOF
                "Melon", "Melon", "Melon",  // 1.00 (pair + single)
                
                // Limes with 3 for 2
                "Lime", "Lime", "Lime", "Lime", // 0.45 (three for two + single)
                
                // Regular items
                "Apple", "Apple",  // 0.70
                "Banana"          // 0.20
            };

            // Total should be 2.35

            // Act
            var total = _basket.CalculateTotal(items);

            // Assert
            Assert.That(total, Is.EqualTo(2.35m));
        }

        [Test]
        public void NullList_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.That(() => _basket.CalculateTotal(null),
                Throws.ArgumentNullException);
        }

        [Test]
        public void SingleItemQuantities_CalculatesCorrectly()
        {
            // Testing each item individually
            Assert.Multiple(() =>
            {
                Assert.That(_basket.CalculateTotal(new List<string> { "Apple" }), Is.EqualTo(0.35m));
                Assert.That(_basket.CalculateTotal(new List<string> { "Banana" }), Is.EqualTo(0.20m));
                Assert.That(_basket.CalculateTotal(new List<string> { "Melon" }), Is.EqualTo(0.50m));
                Assert.That(_basket.CalculateTotal(new List<string> { "Lime" }), Is.EqualTo(0.15m));
            });
        }
    }
}