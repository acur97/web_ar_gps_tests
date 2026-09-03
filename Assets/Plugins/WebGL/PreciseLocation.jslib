var LibraryPreciseLocation = {

    $preciseLocation: {
        installed: false,
        latitude: 0.0,
        longitude: 0.0,
        accuracy: 0.0
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
                    preciseLocation.accuracy = position.coords.accuracy;

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

    PreciseLocation_GetAccuracy__deps: ['$preciseLocation'],
    PreciseLocation_GetAccuracy: function () {
        return preciseLocation.accuracy;
    }
};

mergeInto(LibraryManager.library, LibraryPreciseLocation);