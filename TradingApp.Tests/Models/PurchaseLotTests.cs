using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradingApp.Tests.Models {
    public class PurchaseLotTests {
        [Fact]
        public static void Constructor_WithValidParams_ShouldReturnValidPurchaseLot() {
            var lotId = 1;
            var quantity = 10;
            var purchasePrice = 150.25m;
            var purchaseDate = DateTime.UtcNow;

            var purchaseLot = new TradingApp.Models.PurchaseLot(lotId, quantity, purchasePrice, purchaseDate);
            Assert.Equal(lotId, purchaseLot.PurchaseLotId);
            Assert.Equal(quantity, purchaseLot.Quantity);
            Assert.Equal(purchasePrice, purchaseLot.PurchasePrice);
            Assert.Equal(purchaseDate, purchaseLot.PurchaseDate);
        }
    }
}
