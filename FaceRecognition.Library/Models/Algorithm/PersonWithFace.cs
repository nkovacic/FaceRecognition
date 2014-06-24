using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceDetection.Library.Models
{
    public class PersonWithFace
    {
        public SimplePerson PersonOnFace { get; set; }
        public Face FaceForPerson { get; set; }
        public double EigenDistance { get; set; }
        public FaceRecognitionStatus RecognitionStatus { get; set; }
    }
}
