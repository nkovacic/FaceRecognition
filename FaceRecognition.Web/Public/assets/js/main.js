"use strict";

require.config({
    baseUrl: "/public/assets/js/",
    paths: {
        angular: "vendor/angular/angular",
        "angular-file-upload": "vendor/angular-file-upload/angular-file-upload",
        angularRoute: "vendor/angular-route/angular-route",
        angularResource: "vendor/angular-resource/angular-resource",
        jquery: "vendor/jquery/dist/jquery",
        text: "vendor/requirejs-text/text",
        webcam: "vendor/webcam-directive/app/scripts/webcam",
    },
    shim: {
        angular: { 
            deps: [
                "jquery"
            ],
            exports: "angular" 
        },
        "angular-file-upload": {
            deps: [
                "angular"
            ]
        },
        angularRoute: ["angular"],
        angularResource: {
            deps: [
                "angular"
            ]
        },
        webcam: {
            deps: ["angular"]
        }
    },
    priority: [
		"angular"
    ]
});

//http://code.angularjs.org/1.2.1/docs/guide/bootstrap#overview_deferred-bootstrap
window.name = "NG_DEFER_BOOTSTRAP!";

require([
	"angular",
	"app",
	"routes"
], function (angular, app, routes) {
    "use strict";
    var $html = angular.element("html");

    angular.element().ready(function () {
        angular.resumeBootstrap([app["name"]]);
    });
});
