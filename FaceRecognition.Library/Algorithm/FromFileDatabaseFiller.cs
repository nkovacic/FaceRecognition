using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using FaceDetection.Library.Models;
using FaceDetection.Library.ViewModels;
using Ionic.Zip;
using System.Reflection;

namespace FaceDetection.Library.Algorithm
{
    class FromFileDatabaseFiller
    {
        private readonly string _tempDirectory;
        private readonly FaceRecognitionAlgorithm _faceRecognitionAlgorithm;
        public FromFileDatabaseFiller(FaceRecognitionAlgorithm faceRecognitionAlgorithm)
        {
            _faceRecognitionAlgorithm = faceRecognitionAlgorithm;
            _tempDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase).Replace("file:\\", "") + "\\Files\\Temp";
        }

        public Dictionary<string, FaceRecognitionTrainingStatus> ReadImagesWithPersonsFromZip(FileModel zippedDirectory)
        {
            if (zippedDirectory != null && zippedDirectory.Content != null && ZipFile.IsZipFile(zippedDirectory.Content, false))
            {
                if (zippedDirectory.Content.CanSeek)
                {
                    zippedDirectory.Content.Seek(0, SeekOrigin.Begin);
                }

                string extractedPath = _tempDirectory + "\\" + Guid.NewGuid();

                if (!Directory.Exists(_tempDirectory))
                {
                    Directory.CreateDirectory(_tempDirectory);
                }

                using (var zipFile = ZipFile.Read(zippedDirectory.Content))
                {
                    zipFile.ExtractAll(extractedPath, ExtractExistingFileAction.OverwriteSilently);                  
                }

                var faceRecognitionTrainingStatuses = ReadImagesWithPersonsFromDirectory(extractedPath);
                
                Directory.Delete(extractedPath, true);

                return faceRecognitionTrainingStatuses;
                //zipFile.ExtractAll()
            }

            return null;
        }

        public Dictionary<string, FaceRecognitionTrainingStatus> ReadImagesWithPersonsFromDirectory(string directoryWithImages)
        {
            var faceRecognitionTrainingStatuses = new Dictionary<string, FaceRecognitionTrainingStatus>();

            if (!Directory.Exists(directoryWithImages))
            {
                faceRecognitionTrainingStatuses.Add("failure", FaceRecognitionTrainingStatus.TrainingFailure);

                return faceRecognitionTrainingStatuses;
            }

            var directoryWithImagesInfo = new DirectoryInfo(directoryWithImages);

            foreach (var personDirectoryInfo in directoryWithImagesInfo.GetDirectories())
            {
                var personDirectoryInfoSplitted = personDirectoryInfo.Name.Split(' ');

                if (personDirectoryInfoSplitted.Length != 2)
                {
                    faceRecognitionTrainingStatuses.Add(personDirectoryInfo.Name, FaceRecognitionTrainingStatus.TrainingFailure);

                    return faceRecognitionTrainingStatuses;
                }

                var simplePersonFromDirectoryInfo = new SimplePerson
                {
                    FirstName = personDirectoryInfoSplitted.First(),
                    LastName = personDirectoryInfoSplitted.Last()
                };

                foreach (var imageFileInfo in personDirectoryInfo.GetFiles())
                {
                    try
                    {
                        using (var imageWithFace = new Bitmap(imageFileInfo.FullName))
                        {
                            var faceRecognitionTrainingStatus = _faceRecognitionAlgorithm.AddImageToTrainingSet(imageWithFace, simplePersonFromDirectoryInfo);

                            faceRecognitionTrainingStatuses.Add(imageFileInfo.Name, faceRecognitionTrainingStatus);
                        }
                        
                    }
                    catch (Exception e)
                    {
                        faceRecognitionTrainingStatuses.Add(imageFileInfo.Name, FaceRecognitionTrainingStatus.TrainingFailure);
                        throw e;
                    }
                }
            }

            return faceRecognitionTrainingStatuses;
        }
    }
}
