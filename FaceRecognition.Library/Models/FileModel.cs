using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace FaceDetection.Library.ViewModels
{
    public class FileModel
    {
        public string Filename { get; set; }

        public int ContentLength { get; set; }

        public string ContentType { get; set; }

        public Stream Content { get; set; }

        public FileModel() { }

        public FileModel(string filename, string contentType, int contentLength, Stream content)
        {
            Filename = filename;
            ContentLength = contentLength;
            Content = content;
            ContentType = contentType;
        }
    }
}