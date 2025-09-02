window.tradingView = {
    loadSingleQuote: function (containerId, symbol, options) {
        const host = document.getElementById(containerId);
        if (!host) return;

        // Clear previous render
        host.innerHTML = "";

        // TradingView requires a script tag with JSON config as its innerHTML
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

        // Important: config must be inside the script tag content
        s.innerHTML = JSON.stringify(cfg);

        // The TradingView script reads from its parent container
        (host.parentElement || host).appendChild(s);
    }
};
