using FaceDetection.Library.DataAcccessLayer;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceDetection.Library.Migrations
{
    class DbSeeder : DropCreateDatabaseIfModelChanges<FaceDetectionContext>
    {
        public DbSeeder()
        {

        }

        protected override void Seed(FaceDetectionContext context)
        {
        }
    }
}
