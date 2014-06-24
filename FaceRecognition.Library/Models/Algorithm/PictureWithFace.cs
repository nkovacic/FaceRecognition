using Emgu.CV;
using Emgu.CV.Structure;

namespace FaceDetection.Library.Models
{
    class PictureWithFace
    {
        public Image<Gray, byte> NormalizedPicture { get; set; }
        public Face FaceOnPicture { get; set; }
    }
}
