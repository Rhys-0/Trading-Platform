using System.Collections.Generic;
using System.Text.Json;

namespace TradingApp.BackgroundServices {
    internal static class FinnhubParser {
        internal static IEnumerable<(string symbol, decimal price)> ParseTrades(string json) {
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (!root.TryGetProperty("type", out var typeEl) || typeEl.GetString() != "trade")
                yield break;

            if (!root.TryGetProperty("data", out var dataEl) || dataEl.ValueKind != JsonValueKind.Array)
                yield break;

            foreach (var t in dataEl.EnumerateArray()) {
                if (t.TryGetProperty("s", out var sEl) &&
                    t.TryGetProperty("p", out var pEl) &&
                    sEl.ValueKind == JsonValueKind.String &&
                    pEl.ValueKind == JsonValueKind.Number) {
                    yield return (sEl.GetString()!, pEl.GetDecimal());
                }
            }
        }
    }
}
