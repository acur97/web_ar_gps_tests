var LibraryPreciseLocation = {

    $preciseLocation: {
        installed: false,
        latitude: 0.0,
        longitude: 0.0,

        alpha: 0.0,
        beta: 0.0,
        gamma: 0.0
    },

    PreciseLocation_Install__deps: ['$preciseLocation'],
    PreciseLocation_Install: function () {

        if (preciseLocation.installed)
            return;

        const options = {
            enableHighAccuracy: true,
            maximumAge: 0
        };

        const originalWatchPosition =
            navigator.geolocation.watchPosition.bind(navigator.geolocation);

        navigator.geolocation.watchPosition = function (success, error, options) {

            return originalWatchPosition(
                function (position) {

                    preciseLocation.latitude = position.coords.latitude;
                    preciseLocation.longitude = position.coords.longitude;

                    success(position);
                },
                error,
                options
            );
        };

        preciseLocation.installed = true;
    },

    PreciseLocation_GetLatitude__deps: ['$preciseLocation'],
    PreciseLocation_GetLatitude: function () {
        return preciseLocation.latitude;
    },

    PreciseLocation_GetLongitude__deps: ['$preciseLocation'],
    PreciseLocation_GetLongitude: function () {
        return preciseLocation.longitude;
    },

    PreciseCompass_GetAlpha__deps: ['$preciseLocation'],
    PreciseCompass_GetAlpha: function () {
        return preciseLocation.alpha;
    },

    PreciseCompass_GetBeta__deps: ['$preciseLocation'],
    PreciseCompass_GetBeta: function () {
        return preciseLocation.beta;
    },

    PreciseCompass_GetGamma__deps: ['$preciseLocation'],
    PreciseCompass_GetGamma: function () {
        return preciseLocation.gamma;
    }
};

mergeInto(LibraryManager.library, LibraryPreciseLocation);