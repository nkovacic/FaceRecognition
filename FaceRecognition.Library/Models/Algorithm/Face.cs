using FaceDetection.Library.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace FaceDetection.Library.Models
{
    public class Face
    {
        public Rectangle Bounds { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }

        public int SquarePixels
        {
            get
            {
                return Bounds.Width * Bounds.Height;
            }
        }

        public Face(Rectangle bounds)
        {
            Bounds = bounds;
            X = bounds.X;
            Y = bounds.Y;
            Height = bounds.Height;
            Width = bounds.Width;
        }
    }
}
