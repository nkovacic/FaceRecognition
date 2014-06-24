using FaceDetection.Library.DataAcccessLayer;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FaceDetection.Library.DataAcccessLayer
{
    class EntityBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

        public EntityBase()
        {
            Created = DateTime.Now;
            Updated = Created;
        }
    }
}
