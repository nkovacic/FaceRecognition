define(["angular", "services", "webcam", "angular-file-upload"], function (angular) {
    "use strict";

    /* Controllers */

    return angular.module("faceDetection.controllers", ["faceDetection.services", "webcam", "angularFileUpload"])
		// Sample controller where service is being used
		/*.controller("MyCtrl1", ["$scope", "version", function ($scope, version) {
		    $scope.scopedAppVersion = version;
		}])*/
		// More involved example where controller is required from an external file
		.controller("CameraController", ["$scope", "$injector", function ($scope, $injector) {
		    require(["controllers/CameraController"], function (CameraController) {
		        // injector method takes an array of modules as the first argument
		        // if you want your controller to be able to use components from
		        // any of your other modules, make sure you include it together with "ng"
		        // Furthermore we need to pass on the $scope as it"s unique to this controller
		        $injector.invoke(CameraController, this, { "$scope": $scope });
		    });
		}])
        .controller("TrainingController", ["$scope", "$injector", function ($scope, $injector) {
            require(["controllers/TrainingController"], function (TrainingController) {
                // injector method takes an array of modules as the first argument
                // if you want your controller to be able to use components from
                // any of your other modules, make sure you include it together with "ng"
                // Furthermore we need to pass on the $scope as it"s unique to this controller
                $injector.invoke(TrainingController, this, { "$scope": $scope });
            });
        }]);
});