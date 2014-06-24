using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceDetection.Library.Utilities
{
    class FileHelper
    {
        public static void Save(Image<Bgr, Byte> img, string filename, double quality)
        {
            var encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = new EncoderParameter(
                System.Drawing.Imaging.Encoder.Quality,
                (long)quality
                );

            var jpegCodec = (from codec in ImageCodecInfo.GetImageEncoders()
                             where codec.MimeType == "image/jpeg"
                             select codec).Single();

            img.Bitmap.Save(filename, jpegCodec, encoderParams);
        }

        public static void Save(Image<Gray, Byte> img, string filename, double quality)
        {
            var encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = new EncoderParameter(
                System.Drawing.Imaging.Encoder.Quality,
                (long)quality
                );

            var jpegCodec = (from codec in ImageCodecInfo.GetImageEncoders()
                             where codec.MimeType == "image/jpeg"
                             select codec).Single();

            img.Bitmap.Save(filename, jpegCodec, encoderParams);
        }

        public static string ImageExtensionFromImageFormat(ImageFormat imageFormat)
        {
            if (ImageFormat.Jpeg.Equals(imageFormat))
            {
                return ".jpg";
            }
            else if (ImageFormat.Gif.Equals(imageFormat))
            {
                return ".gif";
            }
            else if (ImageFormat.Png.Equals(imageFormat))
            {
                return ".png";
            }
            else if (ImageFormat.Bmp.Equals(imageFormat))
            {
                return ".bmp";
            }

            return ".jpg";
        }
    }
}
