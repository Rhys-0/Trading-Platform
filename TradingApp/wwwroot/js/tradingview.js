(function () {
    const root = window;
    root.tradingView = root.tradingView || {};

    root.tradingView.loadSymbolOverview = root.tradingView.loadSymbolOverview || function (containerId, symbols, options) {
        const host = document.getElementById(containerId);
        if (!host) return;

        host.innerHTML = "";

        const s = document.createElement("script");
        s.type = "text/javascript";
        s.src = "https://s3.tradingview.com/external-embedding/embed-widget-symbol-overview.js";
        s.async = true;

        const list = Array.isArray(symbols) ? symbols : [];
        const pairs = list.map(sym => {
            const label = (sym && sym.includes(":")) ? sym.split(":")[1] : (sym || "Symbol");
            return [label, `${sym}|1D`];
        });

        const cfg = {
            lineWidth: 2,
            lineType: 0,
            chartType: "area",
            fontColor: "rgb(106, 109, 120)",
            gridLineColor: "rgba(46, 46, 46, 0.06)",
            volumeUpColor: "rgba(34, 171, 148, 0.5)",
            volumeDownColor: "rgba(247, 82, 95, 0.5)",
            backgroundColor: "#ffffff",
            widgetFontColor: "#0F0F0F",
            upColor: "#22ab94",
            downColor: "#f7525f",
            borderUpColor: "#22ab94",
            borderDownColor: "#f7525f",
            wickUpColor: "#22ab94",
            wickDownColor: "#f7525f",
            colorTheme: options?.colorTheme || "light",
            isTransparent: !!options?.isTransparent,
            locale: options?.locale || "en",
            chartOnly: false,
            scalePosition: "right",
            scaleMode: "Normal",
            fontFamily: "-apple-system, BlinkMacSystemFont, Trebuchet MS, Roboto, Ubuntu, sans-serif",
            valuesTracking: "1",
            changeMode: "price-and-percent",
            symbols: pairs.length ? pairs : [["AAPL", "NASDAQ:AAPL|1D"]],
            dateRanges: ["1d|1", "1m|30", "3m|60", "12m|1D", "60m|1W", "all|1M"],
            fontSize: "10",
            headerFontSize: "medium",
            autosize: true,
            width: "100%",
            height: "100%",
            noTimeScale: false,
            hideDateRanges: false,
            hideMarketStatus: false,
            hideSymbolLogo: false
        };

        s.innerHTML = JSON.stringify(cfg);
        host.appendChild(s);
    };

    root.tradingViewReady = function () {
        return !!(root.tradingView && typeof root.tradingView.loadSymbolOverview === "function");
    };

    console.log("[TV] tradingview.js loaded");
})();
