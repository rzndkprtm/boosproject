const CACHE_NAME = "preload-cache-v1";

// Preload manifest
async function preloadAssets() {
    try {
        const response = await fetch("/assets/manifest.json?ts=" + Date.now());
        const manifest = await response.json();

        const allAssets = []
            .concat(manifest.scripts || [])
            .concat(manifest.styles || [])
            .concat(manifest.images || []);

        const cache = await caches.open(CACHE_NAME);
        for (const asset of allAssets) {
            try {
                await cache.add(asset);
                self.clients.matchAll().then(clients => {
                    clients.forEach(client => {
                        client.postMessage({ type: "preload-progress", asset });
                    });
                });
            } catch (err) { /* skip */ }
        }

        self.clients.matchAll().then(clients => {
            clients.forEach(client => {
                client.postMessage({ type: "preload-complete" });
            });
        });

    } catch (err) {
        console.error("Manifest load failed:", err);
    }
}

// Install event = preload blocking
self.addEventListener("install", event => {
    event.waitUntil(preloadAssets());
    self.skipWaiting();
});

// Activate
self.addEventListener("activate", event => {
    clients.claim();
});

// Fetch handler (SWR)
self.addEventListener("fetch", event => {
    event.respondWith(
        caches.match(event.request).then(cached => {
            const fetchPromise = fetch(event.request)
                .then(networkResponse => {
                    caches.open(CACHE_NAME).then(cache => {
                        cache.put(event.request, networkResponse.clone());
                    });
                    return networkResponse;
                })
                .catch(() => cached);

            return cached || fetchPromise;
        })
    );
});
