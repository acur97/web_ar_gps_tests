var LibraryPreciseLocation = {

    $preciseLocation: {
        installed: false,
        latitude: 0.0,
        longitude: 0.0,
        altitude: 0.0
    },

    PreciseLocation_Install__deps: ['$preciseLocation'],
    PreciseLocation_Install: function () {

        if (preciseLocation.installed)
            return;

        const originalWatchPosition =
            navigator.geolocation.watchPosition.bind(navigator.geolocation);

        navigator.geolocation.watchPosition = function (success, error, options) {

            return originalWatchPosition(
                function (position) {

                    preciseLocation.latitude = position.coords.latitude;
                    preciseLocation.longitude = position.coords.longitude;
                    preciseLocation.altitude = position.coords.altitude;

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

    PreciseLocation_GetAltitude__deps: ['$preciseLocation'],
    PreciseLocation_GetAltitude: function () {
        return preciseLocation.altitude;
    }
};

mergeInto(LibraryManager.library, LibraryPreciseLocation);