define(["angular"], function (angular) {
    "use strict";

    return ["$scope", "$http", "$fileUploader", "Detection", function ($scope, $http, $fileUploader, Detection) {
        var _videoElement = null,
            _videoStream = null,
            _getVideoData = function getVideoData() {
                var hiddenCanvas = document.createElement('canvas');
                hiddenCanvas.width = _videoElement.width;
                hiddenCanvas.height = _videoElement.height;
                var ctx = hiddenCanvas.getContext('2d');
                ctx.drawImage(_videoElement, 0, 0, _videoElement.width, _videoElement.height);
                return ctx.getImageData(0, 0, _videoElement.width, _videoElement.height);
            },
            _getImagedataFromSnapshotCanvas = function () {
                var snapshotCanvas = angular.element(".face-detection-processed")[0],
                snapshotCanvasCtx = snapshotCanvas.getContext('2d'),
                imageData = snapshotCanvas.toDataURL("image/png").replace(/^data:image\/(png|jpg);base64,/, "");

                return imageData;
            },
            _showBase64Image = function (base64String, canvas) {
                var onLoadFile = function (base64String) {
                    var img = new Image();
                    img.onload = onLoadImage;
                    img.src = base64String;
                },
                onLoadImage = function () {
                    var maxWidth = canvas.parent().width(),
                        width = this.width, //this.width / this.height * params.height;
                        height = this.height,
                        ratio = width / height; //this.height / this.width * params.width;

                    if (width > maxWidth) {
                        width = maxWidth;
                        height = width / ratio;
                    }

                    canvas.attr({
                        width: width,
                        height: height
                    });
                    canvas[0].getContext("2d").drawImage(this, 0, 0, width, height);
                };

                onLoadFile(base64String);
            };

        // You can access the scope of the controller from here
        $scope.webcamError = false;
        $scope.onError = function (err) {
            console.log("nekaj");

            $scope.webcamError = err;
        };
        $scope.onSuccess = function (videoElement) {

        };

        $scope.onStream = function (videoStream, videoElement) {
            // The video element contains the captured camera data
            _videoStream = videoStream;
            _videoElement = videoElement;
        };

        $scope.onTakeSnapshotClick = function (e) {
            if (_videoElement) {
                var snapshotCanvas = angular.element(".face-detection-processed")[0];

                if (!snapshotCanvas) {
                    return;
                }

                var snapshotCanvasCtx = snapshotCanvas.getContext('2d'),
                    pictureData = _getVideoData();

                snapshotCanvas.width = _videoElement.width;
                snapshotCanvas.height = _videoElement.height;           
                snapshotCanvasCtx.putImageData(pictureData, 0, 0);
                $scope.showTrainingPanel = true;
                /*
                $scope.person = {
                    firstName: "Niko",
                    lastName: "Kovačič"
                };*/
            }
        }

        $scope.onStopVideoClick = function (e) {
            if (_videoStream) {
                _videoStream.stop();
            }
        }
        
        $scope.onEndTrainingClick = function (e) {
            $scope.showTrainingPanel = false;
            $scope.trainingError = false;
            $scope.trainingSuccess = false;

            var newDetection = new Detection({
                imageBase64: _getImagedataFromSnapshotCanvas(),
                person: $scope.person
            });

            Detection
                .train(newDetection)
                .$promise
                .then(
                    function (data) {
                        $scope.trainingError = false;
                        $scope.trainingSuccess = {
                            message: "Treniranje je bilo uspešno"
                        };
                    },
                    function ($error) {
                        $scope.trainingError = {
                            message: $error.data.message
                        };
                        $scope.trainingSuccess = false;
                    }
                );
        };


        var uploader = $scope.uploader = $fileUploader.create({
            autoUpload: true,
            removeAfterUpload: true,
            url: "api/detection/training",
            scope: $scope
        });

        uploader.bind("afteraddingfile", function (event, item) {
            if (item.file.name.indexOf(".zip") != -1) {
                uploader.addtoQueue(item.file);
            }
            else {
                var reader = new FileReader(),
                canvas = angular.element(".face-detection-processed"),
                onLoadFile = function (e) {
                    _showBase64Image(e.target.result, canvas);
                    $scope.showTrainingPanel = true;
                };

                reader.onload = onLoadFile;
                reader.readAsDataURL(item.file);
            }
        });

        uploader.bind("completeall", function (event, items) {

        });
    }];
});