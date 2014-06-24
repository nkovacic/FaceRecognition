using FaceDetection.Library.Models;
using FaceDetection.Web.ViewModels.Detection;
using System;
using System.Collections.Generic;

namespace FaceDetection.Web.ViewModels.Recognition
{
    public class TrainViewModel : DetectionViewModel
    {
        public SimplePerson Person { get; set; }

        public TrainViewModel() : base() { }
    }
}