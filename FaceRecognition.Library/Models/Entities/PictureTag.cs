using FaceDetection.Library.DataAcccessLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace FaceDetection.Library.Models
{
    [Table("PictureTags")]
    class PictureTag : EntityBase
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public virtual ICollection<Picture> OwnerPictures { get; set; }
        public virtual Person PersonOnImageTag { get; set; }

        public PictureTag() { }

        public PictureTag(PictureTag pictureTag)
        {
            X = pictureTag.X;
            Y = pictureTag.Y;
            Height = pictureTag.Height;
            Width = pictureTag.Width;
            PersonOnImageTag = pictureTag.PersonOnImageTag;
        }
    }
}
