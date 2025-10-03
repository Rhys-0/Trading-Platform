export async function ensureTvLoaded(version = 1) {
    if (window.tradingView?.loadSymbolOverview) return true;

    await new Promise((resolve, reject) => {
        const s = document.createElement("script");
        s.src = `/js/tradingview.js?v=${version}`;
        s.async = false;
        s.onload = () => resolve();
        s.onerror = () => reject(new Error("Failed to load tradingview.js"));
        document.head.appendChild(s);
    });

    return !!(window.tradingView?.loadSymbolOverview);
}

export function renderOverview(containerId, symbols, options) {
    if (!window.tradingView?.loadSymbolOverview) {
        throw new Error("TradingView not ready");
    }
    window.tradingView.loadSymbolOverview(containerId, symbols, options);
}
