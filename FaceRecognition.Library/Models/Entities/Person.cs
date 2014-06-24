using FaceDetection.Library.DataAcccessLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace FaceDetection.Library.Models
{
    [Table("Persons")]
    class Person : EntityBase
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public ICollection<PictureTag> TaggedPictures { get; set; }

        public SimplePerson ToSimplePerson()
        {
            return new SimplePerson
            {
                FirstName = FirstName,
                LastName = LastName
            };
        }

        public PersonWithFace ToPersonWithFace()
        {
            return new PersonWithFace
            {
                PersonOnFace = ToSimplePerson()
            };
        }
    }
}
