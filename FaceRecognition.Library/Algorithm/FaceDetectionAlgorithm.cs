using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using FaceDetection.Library.Models;
using FaceDetection.Library.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceDetection.Library.Algorithm
{
    class FaceDetectionAlgorithm
    {
        private const double _scale = 1;
        private const double _scaleFactorFirst = 1.1;
        private const double _scaleFactorSecond = 1.01;
        private const int _minNeighborsFirst = 10;
        private const int _minNeighborsSecond = 4;

        private readonly CascadeClassifier _faceHaarCascade;
        private readonly CascadeClassifier _eyeHaarCascade;

        public FaceDetectionAlgorithm()
        {
            string path = new Uri(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase) + "\\Files\\Cascades").LocalPath+"\\";

            _faceHaarCascade = new CascadeClassifier(path + "haarcascade_frontalface_alt.xml");
            _eyeHaarCascade = new CascadeClassifier(path + "haarcascade_eye.xml");
        }

        public FaceDetectionAlgorithm(string faceHaarCascadePath, string eyeHaarCascadePath)
        {
            _faceHaarCascade = new CascadeClassifier(faceHaarCascadePath);
            _eyeHaarCascade = new CascadeClassifier(eyeHaarCascadePath);
        }

        public List<Face> FindFaces(Bitmap image)
        {
            var results = new List<Face>();

            using (var img = new Image<Bgr, byte>(image))
            {
                var grayImage = img.Convert<Gray, Byte>();
                grayImage._EqualizeHist();

                var detectedFaces = DetectObjectsWithHaar(grayImage, _scaleFactorFirst, _minNeighborsSecond, false);

                results.AddRange(detectedFaces);
            }

            var filteredResults = new List<Face>();

            while (results.Count > 0)
            {
                var face = results.First();
                results.RemoveAt(0);

                foreach (var otherFace in results.FindAll(f => f.Bounds.IntersectsWith(face.Bounds)))
                {
                    int left = Math.Min(face.Bounds.Left, otherFace.Bounds.Left);
                    int top = Math.Min(face.Bounds.Top, otherFace.Bounds.Top);
                    int right = Math.Max(face.Bounds.Right, otherFace.Bounds.Right);
                    int bottom = Math.Max(face.Bounds.Bottom, otherFace.Bounds.Bottom);

                    face.Bounds = new Rectangle(new Point(left, top), new Size(right - left, bottom - top));
                    results.Remove(otherFace);
                }

                filteredResults.Add(face);
            }

            return filteredResults;
        }

        private IEnumerable<Face> DetectObjectsWithHaar(Image<Gray, byte> image, double scaleFactor, int minNeighbors, bool checkForEyes = false)
        {
            var faces = _faceHaarCascade.DetectMultiScale(image, scaleFactor, minNeighbors, new Size(20, 20), Size.Empty);

            if (checkForEyes)
            {
                var facesWithEyes = new List<Rectangle>();
                foreach (var face in faces)
                {
                    image.ROI = face;
                    var eyes = _eyeHaarCascade.DetectMultiScale(image, scaleFactor, minNeighbors, new Size(20, 20), Size.Empty);
                    image.ROI = new Rectangle(0, 0, image.Width, image.Height);

                    if (eyes.Count() > 0)
                    {
                        facesWithEyes.Add(face);
                    }
                }

                return facesWithEyes.Select(face => new Face(face));
            }
            else
            {
                return faces.Select(face => new Face(face));
            }
        }
    }
}
