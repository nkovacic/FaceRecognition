using FaceDetection.Library.DataAcccessLayer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using FaceDetection.Library.Algorithm;
using FaceDetection.Library.Models;
using System.Data.Entity;
using FaceDetection.Library.Migrations;
using FaceDetection.Library.ViewModels;

namespace FaceDetection.Library
{
    public class FaceRecognitionLibrary
    {
        private const double _scale = 1;
        private const double _scaleFactorFirst = 1.05;
        private const double _scaleFactorSecond = 1.01;
        private const int _minNeighborsFirst = 50;
        private const int _minNeighborsSecond = 4;

        private readonly FaceDetectionAlgorithm _faceDetectionAlgorithm;
        private readonly FaceRecognitionAlgorithm _faceRecognitionAlgorithm;
        private readonly FromFileDatabaseFiller _fromFileDatabaseFiller;

        public FaceRecognitionLibrary()
        {
            
        }

        public FaceRecognitionLibrary(string nameOrConnectionString) : this()
        {
            _faceDetectionAlgorithm = new FaceDetectionAlgorithm();
            _faceRecognitionAlgorithm = new FaceRecognitionAlgorithm(nameOrConnectionString, _faceDetectionAlgorithm);
            _fromFileDatabaseFiller = new FromFileDatabaseFiller(_faceRecognitionAlgorithm);
        }

        public FaceRecognitionLibrary(string faceHaarCascadePath, string eyeHaarCascadePath, string nameOrConnectionString)
            : this()
        {
            _faceDetectionAlgorithm = new FaceDetectionAlgorithm(faceHaarCascadePath, eyeHaarCascadePath);
            _faceRecognitionAlgorithm = new FaceRecognitionAlgorithm(nameOrConnectionString, _faceDetectionAlgorithm);
        }

        public List<Face> FindFaces(Bitmap image)
        {
            return _faceDetectionAlgorithm.FindFaces(image);
        }
        
        public Bitmap DrawFacesBoundsOnBitmap(Bitmap image, List<Face> faces)
        {
            foreach (var face in faces)
            {
                image = DrawFaceBoundsOnBitmap(image, face);
            }

            return image;
        }
        public Bitmap DrawFaceBoundsOnBitmap(Bitmap image, Face face)
        {
            using (var graphics = Graphics.FromImage(image))
            {
                using (var pen = new Pen(Color.White, 2.0f))
                {
                    graphics.DrawRectangle(pen, face.Bounds.X, face.Bounds.Y, face.Bounds.Width, face.Bounds.Height);
                }
            }

            return image;
        }

        public List<PersonWithFace> FaceRecognition(Bitmap image)
        {
            return _faceRecognitionAlgorithm.Recognize(image);
        }

        public FaceRecognitionTrainingStatus TrainFaceRecognition(Bitmap image, SimplePerson personOnImage)
        {
            return _faceRecognitionAlgorithm.AddImageToTrainingSet(image, personOnImage);
        }

        public Dictionary<string, FaceRecognitionTrainingStatus> TrainFaceRecognition(FileModel zippedDirectory)
        {
            return _fromFileDatabaseFiller.ReadImagesWithPersonsFromZip(zippedDirectory);
        }

        public Dictionary<string, FaceRecognitionTrainingStatus> TrainFaceRecognition(string directoryPath)
        {
            return _fromFileDatabaseFiller.ReadImagesWithPersonsFromDirectory(directoryPath);
        }
    }
}
