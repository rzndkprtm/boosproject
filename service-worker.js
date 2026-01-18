const CACHE_VERSION = "v3";
const PRELOAD_CACHE = `preload-${CACHE_VERSION}`;
const RUNTIME_CACHE = `runtime-${CACHE_VERSION}`;

async function preloadAssets() {
    try {
        const response = await fetch("/assets/manifest.json?ts=" + Date.now());
        const manifest = await response.json();

        const critical = []
            .concat(manifest.critical?.scripts || [])
            .concat(manifest.critical?.styles || []);

        const optional = []
            .concat(manifest.optional?.scripts || [])
            .concat(manifest.optional?.styles || [])
            .concat(manifest.images || []);

        const cache = await caches.open(PRELOAD_CACHE);

        // parallel preload critical
        await Promise.all(
            critical.map(asset =>
                cache.add(asset).then(() => {
                    notifyClient("preload-progress", asset);
                }).catch(() => null)
            )
        );

        notifyClient("preload-critical-complete");

        // optional non-blocking
        optional.forEach(asset => {
            cache.add(asset).then(() => {
                notifyClient("preload-progress", asset);
            }).catch(() => null);
        });

        notifyClient("preload-complete");

    } catch (err) {
        console.error("Manifest preload failed:", err);
    }
}

function notifyClient(type, asset = null) {
    self.clients.matchAll().then(clients => {
        clients.forEach(client => {
            client.postMessage({ type, asset });
        });
    });
}

self.addEventListener("install", event => {
    event.waitUntil(preloadAssets());
    self.skipWaiting();
});

self.addEventListener("activate", event => {
    event.waitUntil(
        caches.keys().then(keys =>
            Promise.all(
                keys.filter(key =>
                    ![PRELOAD_CACHE, RUNTIME_CACHE].includes(key)
                ).map(key => caches.delete(key))
            )
        )
    );
    self.clients.claim();
});

// Runtime cache: Stale-While-Revalidate
self.addEventListener("fetch", event => {
    const req = event.request;

    // bypass no-cache requests
    if (req.cache === "only-if-cached" && req.mode !== "same-origin") {
        return;
    }

    event.respondWith(
        caches.match(req).then(cached => {
            const fetchPromise = fetch(req)
                .then(networkRes => {
                    caches.open(RUNTIME_CACHE).then(cache => {
                        cache.put(req, networkRes.clone());
                    });
                    return networkRes;
                })
                .catch(() => cached);

            return cached || fetchPromise;
        })
    );
});
