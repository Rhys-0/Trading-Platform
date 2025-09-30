const TV_GLOBAL_URL = "/js/tradingview.js?v=14";

function hasTradingView() {
    return !!(window.tradingView && typeof window.tradingView.getQuotesJson === "function");
}

function loadScriptOnce(src) {
    return new Promise((resolve, reject) => {
        if (document.querySelector(`script[data-tv-src="${src}"]`)) {
            resolve();
            return;
        }
        const s = document.createElement("script");
        s.type = "text/javascript";
        s.src = src;
        s.async = true;
        s.dataset.tvSrc = src;
        s.onload = () => resolve();
        s.onerror = () => reject(new Error("Failed to load " + src));
        document.head.appendChild(s);
    });
}

export async function ensureReady() {
    if (hasTradingView()) return true;
    try {
        await loadScriptOnce(TV_GLOBAL_URL);
    } catch (e) {
        console.error("[tv-bridge] script load failed:", e);
        return false;
    }
    return hasTradingView();
}

export async function ready() {
    return await ensureReady();
}

export async function getQuotesJson(symbols) {
    const ok = await ensureReady();
    if (!ok) return "";
    try {
        return window.tradingView.getQuotesJson(symbols);
    } catch (e) {
        console.error("[tv-bridge] getQuotesJson error:", e);
        return "";
    }
}

export async function loadSingleQuote(containerId, symbol, options) {
    const ok = await ensureReady();
    if (!ok) return;
    try {
        window.tradingView.loadSingleQuote(containerId, symbol, options || {});
    } catch (e) {
        console.error("[tv-bridge] loadSingleQuote error:", e);
    }
}
