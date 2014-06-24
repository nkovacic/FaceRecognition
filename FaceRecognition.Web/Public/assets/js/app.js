define(["angular", "filters", "services", "directives", "controllers", "angularRoute", "angularResource"],
    function (angular, filters, services, directives, controllers) {
    "use strict";

    // Declare app level module which depends on filters, and services

    return angular.module("faceDetection", [
        "ngRoute",
        "ngResource",
        "faceDetection.controllers",
        "faceDetection.filters",
        "faceDetection.services",
        "faceDetection.directives"
    ]);
});