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
            _clearCanvas = function (canvas, canvasContext) {
                canvasContext.save();

                // Use the identity matrix while clearing the canvas
                canvasContext.setTransform(1, 0, 0, 1, 0, 0);
                canvasContext.clearRect(0, 0, canvas.width, canvas.height);

                // Restore the transform
                canvasContext.restore();
            },
            _getImagedataFromSnapshotCanvas = function () {
                var snapshotCanvas = angular.element(".face-detection-snapshot")[0],
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
                var snapshotCanvas = angular.element(".face-detection-snapshot")[0];

                if (!snapshotCanvas) {
                    return;
                }

                snapshotCanvas.width = _videoElement.width;
                snapshotCanvas.height = _videoElement.height;
                var snapshotCanvasCtx = snapshotCanvas.getContext('2d'),
                    pictureData = _getVideoData();
                snapshotCanvasCtx.putImageData(pictureData, 0, 0);
            }
        }

        $scope.onStopVideoClick = function (e) {
            if (_videoStream) {
                _videoStream.stop();
            }
        }

        $scope.onDetectFacesClick = function (e) {
            var newDetection = new Detection({
                imageBase64: _getImagedataFromSnapshotCanvas()
            });

            var detections = Detection.detect(newDetection, function (data) {
                var canvas = angular.element(".face-detection-processed");
                data.imageBase64 = "data:image/png;base64," + data.imageBase64;

                _showBase64Image(data.imageBase64, canvas);
            });
        };

        $scope.onFaceRecognitionClick = function (e) {
            var newDetection = new Detection({
                imageBase64: _getImagedataFromSnapshotCanvas()
            }),
                snapshotCanvas = angular.element(".face-detection-snapshot"),
                processedCanvas = angular.element(".face-detection-processed");

            $scope.detectionError = false;
            $scope.detectionSuccess = false;
            $scope.recognitionSuccess = false;
            _clearCanvas(processedCanvas[0], processedCanvas[0].getContext("2d"));

            Detection.recognition(newDetection).$promise.then(
                function (data) {
                    var snapshotContext = snapshotCanvas[0].getContext("2d"),
                        processedCanvasContext = processedCanvas[0].getContext("2d");

                    processedCanvas.attr({
                        width: snapshotCanvas.attr("width"),
                        height: snapshotCanvas.attr("height")
                    });
                    processedCanvasContext.drawImage(snapshotCanvas[0], 0, 0);
                    processedCanvasContext.lineWidth = "4";
                    processedCanvasContext.strokeStyle = "white";


                    angular.forEach(data, function (personWithFace) {
                        processedCanvasContext.strokeRect(personWithFace.faceForPerson.x, personWithFace.faceForPerson.y, personWithFace.faceForPerson.height, personWithFace.faceForPerson.width);
                    });

                    $scope.detectionError = false;
                    $scope.recognitionSuccess = true;
                    $scope.personsWithFace = data;
                },
                function ($error) {
                    $scope.detectionError = {
                        message: $error.data.message
                    };
                }
            );
        };

        var uploader = $scope.uploader = $fileUploader.create({
            scope: $scope
        });

        uploader.bind('afteraddingfile', function (event, item) {
            var reader = new FileReader(),
                canvas = angular.element(".face-detection-snapshot"),
                onLoadFile = function (e) {
                    _showBase64Image(e.target.result, canvas);
                };

            reader.onload = onLoadFile;
            reader.readAsDataURL(item.file);
        });
    }];
});