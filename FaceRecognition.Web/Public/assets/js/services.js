define(["angular"], function (angular) {
    "use strict";

    /* Services */

    // Demonstrate how to register services
    // In this case it is a simple value service.
    angular.module("faceDetection.services", [])
		.factory("Detection", ["$resource",
            function ($resource) {
                return $resource("/api/detection/:action", {}, {
                    detect: {
                        method: "POST",
                        params: {
                            action: "detect"
                        },
                        isArray: false
                    },
                    train: {
                        method: "POST",
                        params: {
                            action: "train"
                        },
                        isArray: false
                    },
                    recognition: {
                        method: "POST",
                        params: {
                            action: "recognition"
                        },
                        isArray: true
                    }
                });
            }
		]);
});