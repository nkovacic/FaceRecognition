using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace FaceDetection.Library.Utilities
{
    class Converter
    {
        /*
        public static CvRect ToCvRect(Rectangle rectangle)
        {
            return new CvRect(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        public static Rectangle ToRectangle(CvRect cvRect)
        {
            return new Rectangle(cvRect.X, cvRect.Y, cvRect.Width, cvRect.Height);
        }
        */
        public static string GetMimeTypeFromImage(Bitmap image)
        {
            var format = image.RawFormat;
            var codec = ImageCodecInfo.GetImageDecoders().FirstOrDefault(c => c.FormatID == format.Guid);

            if (codec != null)
            {
                return codec.MimeType;
            }
            else
            {
                return "image/png";
            }
        }

        public static string GetExtensionFromImage(Bitmap image)
        {
            var format = image.RawFormat;
            var codec = ImageCodecInfo.GetImageDecoders().FirstOrDefault(c => c.FormatID == format.Guid);

            if (codec != null)
            {
                return codec.FilenameExtension;
            }
            else
            {
                return ".png";
            }
        }

        public static byte[, ,] CreateMultiDimensionalBytes(byte[] bytes, int width, int height, int numberOfChannels)
        {
            int oneChannelLength = bytes.Length / numberOfChannels;
            byte[, ,] multiDimensionalBytes = new byte[height, width, numberOfChannels];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    for (int k = 0; k < numberOfChannels; k++)
                    {
                        multiDimensionalBytes[i, j, k] = bytes[i * height + j * numberOfChannels + k];
                    }
                }
            }

            return multiDimensionalBytes;
        }
    }
}
