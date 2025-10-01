window.tradingView = {
    loadSingleQuote: function (containerId, symbol, options) {
        const host = document.getElementById(containerId);
        if (!host) return;

        host.innerHTML = "";

        const s = document.createElement("script");
        s.type = "text/javascript";
        s.src = "https://s3.tradingview.com/external-embedding/embed-widget-single-quote.js";
        s.async = true;

        const cfg = {
            symbol,
            colorTheme: options?.colorTheme || "light",
            isTransparent: !!options?.isTransparent,
            locale: options?.locale || "en",
            width: options?.width || 350
        };

        s.innerHTML = JSON.stringify(cfg);
        (host.parentElement || host).appendChild(s);
    },

    _book: {},

    _seed(symbol) {
        if (!this._book[symbol]) {
            let base = 100;
            if (symbol.includes("TSLA")) base = 250;
            else if (symbol.includes("NVDA")) base = 900;
            else if (symbol.includes("AMZN")) base = 180;
            else if (symbol.includes("AAPL")) base = 185;
            else if (symbol.includes("MSFT")) base = 420;
            else if (symbol.includes("SPY")) base = 510;
            else if (symbol.includes("KO")) base = 60;
            else if (symbol.includes("DIS")) base = 110;
            this._book[symbol] = { price: base, prev: base };
        }
    },

    _tickOne(symbol) {
        this._seed(symbol);
        const row = this._book[symbol];
        row.prev = row.price;

        // random walk +/- ~1%
        const deltaPct = (Math.random() - 0.5) * 0.02;
        const next = row.price * (1 + deltaPct);
        row.price = Math.max(1, Math.round(next * 100) / 100);
    },

    getQuote(symbol) {
        this._tickOne(symbol);
        const row = this._book[symbol];
        const change = +(row.price - row.prev).toFixed(2);
        const changePct = row.prev === 0 ? 0 : +((change / row.prev) * 100).toFixed(2);
        return {
            Symbol: symbol,
            Price: row.price,
            Change: change,
            ChangePercent: changePct,
            IsUp: change >= 0
        };
    },

    getQuotes(symbols) {
        if (!Array.isArray(symbols)) return [];
        return symbols.map(s => this.getQuote(s));
    },

    getQuotesJson(symbols) {
        const json = JSON.stringify(this.getQuotes(symbols));
        console.log("[TV] getQuotesJson called for", symbols, "->", json);
        return json;
    }
};

window.tradingViewReady = function () {
    return !!(window.tradingView && typeof window.tradingView.getQuotes === "function");
};
