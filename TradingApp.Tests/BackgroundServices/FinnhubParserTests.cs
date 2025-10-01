// Tests the JSON -> (symbol, price) extraction only.
// No websockets, no HTTP, just pure parsing.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradingApp.BackgroundServices;
using Xunit;

namespace TradingApp.Tests.BackgroundServices {
    public class FinnhubParserTests {
        [Fact]
        public void ParseTrades_Extracts_AllTrades() {
            const string frame = @"{
              ""type"": ""trade"",
              ""data"": [
                { ""s"": ""AAPL"", ""p"": 256.44 },
                { ""s"": ""TSLA"", ""p"": 456.49 }
              ]
            }";

            var trades = FinnhubParser.ParseTrades(frame).ToArray();

            Assert.Equal(2, trades.Length);
            Assert.Equal(("AAPL", 256.44m), trades[0]);
            Assert.Equal(("TSLA", 456.49m), trades[1]);
        }

        [Fact]
        public void ParseTrades_Ignores_NonTradeFrames() {
            const string frame = @"{ ""type"": ""ping"" }";
            Assert.Empty(FinnhubParser.ParseTrades(frame));
        }
    }
}