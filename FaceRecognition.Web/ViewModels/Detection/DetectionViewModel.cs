using FaceDetection.Web.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace FaceDetection.Web.ViewModels.Detection
{
    public class DetectionViewModel
    {
        public string ImageBase64 { get; set; }

        public DetectionViewModel() { }

        public static DetectionViewModel Build(Bitmap image)
        {
            var detectionViewModel = new DetectionViewModel
            {
                ImageBase64 = ImageUtilities.ImageToBase64(image)
            };

            return detectionViewModel;
        }

        public Bitmap GetImage()
        {
            return ImageUtilities.Base64ToImage(ImageBase64);
        }
    }
}