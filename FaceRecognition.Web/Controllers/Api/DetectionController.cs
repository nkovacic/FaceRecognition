using FaceDetection.Library;
using FaceDetection.Library.Models;
using FaceDetection.Library.ViewModels;
using FaceDetection.Web.Utilities;
using FaceDetection.Web.ViewModels;
using FaceDetection.Web.ViewModels.Detection;
using FaceDetection.Web.ViewModels.Recognition;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace FaceDetection.Web.Controllers
{
    public class DetectionController : ApiController
    {
        private readonly FaceRecognitionLibrary _faceDetectionLibrary;

        public DetectionController(): base()
        {
            //string faceHaarCascadePath = HttpContext.Current.Server.MapPath("~/Files/haarcascade_frontalface_alt.xml");
            //string eyeHaarCascadePath = HttpContext.Current.Server.MapPath("~/Files/haarcascade_eye.xml");
            string connectionString = ConfigurationManager.ConnectionStrings["FaceDetectionContext"].ConnectionString;

            _faceDetectionLibrary = new FaceRecognitionLibrary(connectionString);
        }

        [HttpPost]
        public DetectionViewModel Detect(DetectionViewModel detectionViewModel)
        {
            var faceBitmap = detectionViewModel.GetImage();
            var faces = _faceDetectionLibrary.FindFaces(faceBitmap);

            faceBitmap = _faceDetectionLibrary.DrawFacesBoundsOnBitmap(faceBitmap, faces);
            detectionViewModel = DetectionViewModel.Build(faceBitmap);

            return detectionViewModel;
        }

        [HttpPost]
        public IHttpActionResult Train(TrainViewModel trainViewModel)
        {
            var faceBitmap = trainViewModel.GetImage();
            var faceRecognitionTrainingStatus = _faceDetectionLibrary.TrainFaceRecognition(faceBitmap, trainViewModel.Person);

            switch (faceRecognitionTrainingStatus)
            {
                case FaceDetection.Library.Models.FaceRecognitionTrainingStatus.TrainingSuccessful:
                    return Ok();
                case FaceDetection.Library.Models.FaceRecognitionTrainingStatus.FoundMoreThenOneFace:
                    return BadRequest("Found more then one face.");
                case FaceDetection.Library.Models.FaceRecognitionTrainingStatus.NoFacesFound:
                    return BadRequest("No faces found.");
                case FaceDetection.Library.Models.FaceRecognitionTrainingStatus.TrainingFailure:
                    return BadRequest("Training failure.");
                default:
                    return BadRequest();
            }
        }
        [HttpPost]
        public IHttpActionResult Training(TrainingViewModel zipWithFaces)
        {
            var trainingResult = _faceDetectionLibrary.TrainFaceRecognition(zipWithFaces.File);

            return Ok(trainingResult);
        }

        [HttpPost]
        public IHttpActionResult Recognition(DetectionViewModel detectionViewModel)
        {
            var faceBitmap = detectionViewModel.GetImage();
            var personsOnImage = _faceDetectionLibrary.FaceRecognition(faceBitmap);

            if (personsOnImage == null)
            {
                return BadRequest("No faces found.");
            }
            else if (personsOnImage.Count == 0)
            {
                return BadRequest("Face recognition could not found any persons on image.");
            }
            else if (personsOnImage.Count == 1)
            {
                var personOnImage = personsOnImage.First();

                if (personOnImage.RecognitionStatus == FaceRecognitionStatus.FaceRecognitionFailure 
                    || personOnImage.RecognitionStatus == FaceRecognitionStatus.FaceRecognitionError)
                {
                    if (personOnImage.PersonOnFace == null)
                    {
                        return BadRequest("Face recognition failed with eigen distance " + personOnImage.EigenDistance + ".");
                    }
                    else
                    {
                        return BadRequest("Face recognition failed with eigen distance " + personOnImage.EigenDistance + 
                            ". Presumed person on image is: "+personOnImage.PersonOnFace.FirstName+ " "+ personOnImage.PersonOnFace.LastName);
                    }
                }
            }
            
            return Ok(personsOnImage);
        }
    }
}
