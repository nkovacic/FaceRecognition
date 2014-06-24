using FaceDetection.Library.DataAcccessLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace FaceDetection.Library.Models
{
    [Table("Pictures")]
    class Picture : EntityBase
    {
        public int Height { get; set; }
        public int Width { get; set; }
        public string PicturePath { get; set; }
        public int NumberOfChannels { get; set; }
        public virtual PictureType Type { get; set; }
        public virtual ICollection<PictureTag> Tags { get; set; }
    }
}
