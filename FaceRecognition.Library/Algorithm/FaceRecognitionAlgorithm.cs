using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using FaceDetection.Library.DataAcccessLayer;
using FaceDetection.Library.Models;
using FaceDetection.Library.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace FaceDetection.Library.Algorithm
{
    class FaceRecognitionAlgorithm
    {
        private readonly string _basePicturesPath;
        private const double _eigenDistanceThreshold = 2000;
        private Size _faceSize = new Size(64, 64);
        private FaceRecognitionType _faceRecognitionType;
        private bool _isFaceRecognizerTrained;

        private Image<Gray, byte>[] _trainingImages;
        private Person[] _personsOnImages;

        private FaceRecognizer _faceRecognizer;
        private readonly IUnitOfWork _unitOfWork;
        private readonly FaceDetectionAlgorithm _faceDetectionAlgorithm;

        public double EigenDistanceThreshold
        {
            get { return _eigenDistanceThreshold; }
        }

        public FaceRecognitionAlgorithm()
        {
            _faceRecognitionType = FaceRecognitionType.EigenFaceRecognizer;
            _basePicturesPath = new Uri(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase)).LocalPath;
        }

        public FaceRecognitionAlgorithm(string faceBaseDirectory)
        {
            _faceRecognitionType = FaceRecognitionType.EigenFaceRecognizer;
            _basePicturesPath = faceBaseDirectory;
        }

        public FaceRecognitionAlgorithm(string nameOrConnectionString, FaceDetectionAlgorithm faceDetectionAlgorithm)
            : this()
        {
            _unitOfWork = new UnitOfWork(nameOrConnectionString);
            _faceDetectionAlgorithm = faceDetectionAlgorithm;
        }

        public FaceRecognitionAlgorithm(string nameOrConnectionString, FaceDetectionAlgorithm faceDetectionAlgorithm, string faceBaseDirectory)
            : this(faceBaseDirectory)
        {
            _unitOfWork = new UnitOfWork(nameOrConnectionString);
            _faceDetectionAlgorithm = faceDetectionAlgorithm;
        }

        public FaceRecognitionTrainingStatus AddImageToTrainingSet(Bitmap faceImage, SimplePerson personOnImage)
        {
            using (var originalImage = new Image<Bgr, byte>(faceImage))
            {
                try
                {
                    var faces = _faceDetectionAlgorithm.FindFaces(faceImage);

                    if (faces.Count == 1)
                    {
                        var face = faces.First();

                        var tag = new PictureTag
                        {
                            X = face.Bounds.X,
                            Y = face.Bounds.Y,
                            Height = face.Bounds.Height,
                            Width = face.Bounds.Width,
                        };
                        var existingPerson = _unitOfWork.Repository<Person>().Query(q => q.FirstName == personOnImage.FirstName && q.LastName == personOnImage.LastName).Select().FirstOrDefault();

                        if (existingPerson != null)
                        {
                            tag.PersonOnImageTag = existingPerson;
                        }
                        else
                        {
                            tag.PersonOnImageTag = new Person
                            {
                                FirstName = personOnImage.FirstName,
                                LastName = personOnImage.LastName
                            };
                        }

                        if (!Directory.Exists(_basePicturesPath))
                        {
                            Directory.CreateDirectory(_basePicturesPath);
                        }
                        
                        var thumbnailGrayscaleIplImage = CropAndNormalizeFace(originalImage, face);
                        string originalImageGuid = Guid.NewGuid().ToString();
                        string thumbnailGrayscaleGuid = Guid.NewGuid().ToString();
                        string imageExtension = FileHelper.ImageExtensionFromImageFormat(originalImage.Bitmap.RawFormat);
                        string localOrignalImagePath = _basePicturesPath +"\\Files\\Faces\\"+ originalImageGuid + imageExtension;
                        string localThumbnailGrayscaleImagePath = _basePicturesPath + "\\Files\\Faces\\" + thumbnailGrayscaleGuid + imageExtension;

                        originalImage.Save(localOrignalImagePath);
                        thumbnailGrayscaleIplImage.Save(localThumbnailGrayscaleImagePath);
                        //FileHelper.Save(originalImage, orignalImagePath, 90);
                        //FileHelper.Save(thumbnailGrayscaleIplImage, thumbnailGrayscaleImagePath, 90);

                        var image = new FaceDetection.Library.Models.Image
                        {
                            Pictures = new List<Picture>() 
                            {
                                new Picture
                                {
                                    PicturePath = "/Files/Faces/"+originalImageGuid+imageExtension,
                                    NumberOfChannels = originalImage.NumberOfChannels,
                                    Height = faceImage.Height,
                                    Width = faceImage.Width,
                                    Type = PictureType.Original,
                                    Tags = new List<PictureTag>
                                    {
                                        tag
                                    }
                                },
                                new Picture
                                {
                                    PicturePath = "/Files/Faces/"+thumbnailGrayscaleGuid+imageExtension,
                                    NumberOfChannels = thumbnailGrayscaleIplImage.NumberOfChannels,
                                    Height = _faceSize.Height,
                                    Width = _faceSize.Width,
                                    Type = PictureType.GrayscaleThumbnail,
                                    Tags = new List<PictureTag>
                                    {
                                        tag
                                    }
                                }
                            }
                        };

                        _unitOfWork.BeginTransaction();

                        try
                        {
                            _unitOfWork.Repository<FaceDetection.Library.Models.Image>().Insert(image);
                            _unitOfWork.Commit();

                            return FaceRecognitionTrainingStatus.TrainingSuccessful;
                        }
                        catch (Exception e)
                        {
                            _unitOfWork.Rollback();
                            throw e;
                        }
                    }
                    else if (faces.Count == 0)
                    {
                        return FaceRecognitionTrainingStatus.NoFacesFound;
                    }
                    else
                    {
                        return FaceRecognitionTrainingStatus.FoundMoreThenOneFace;
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
            }

            return FaceRecognitionTrainingStatus.TrainingFailure;
        }

        public List<PersonWithFace> Recognize(Bitmap image)
        {
            var imagesWithNormalizedFaces = GetFaceImages(image);
            var personsOnImage = new List<PersonWithFace>();

            if (imagesWithNormalizedFaces == null)
            {
                return null;
            }

            LoadTrainingImages();
            LoadRecognizer();
            TrainRecognizer();
            
            foreach (var normalizedFaceImage in imagesWithNormalizedFaces)
            {
                var bestPersonMatch = RecognizeFace(normalizedFaceImage.NormalizedPicture);

                if (bestPersonMatch != null)
                {
                    bestPersonMatch.FaceForPerson = normalizedFaceImage.FaceOnPicture;
                    personsOnImage.Add(bestPersonMatch);
                }
            }

            return personsOnImage;
        }

        private List<PictureWithFace> GetFaceImages(Bitmap imageWithFaces)
        {
            var faces = _faceDetectionAlgorithm.FindFaces(imageWithFaces);

            if (faces.Count == 0)
            {
                return null;
            }

            var iplImage = new Image<Bgr, byte>(imageWithFaces);
            var normalizedFaceImages = new List<PictureWithFace>(faces.Count);

            foreach (var face in faces)
            {
                var normalizedFaceImage = CropAndNormalizeFace(iplImage, face);

                normalizedFaceImages.Add(new PictureWithFace
                {
                    NormalizedPicture = normalizedFaceImage,
                    FaceOnPicture = face
                });
            }

            return normalizedFaceImages;
        }

        private Image<Gray, byte> CropAndNormalizeFace(Image<Bgr, byte> inputImage, Face face)
        {
            inputImage.ROI = face.Bounds;

            using (var thumbnailGrayFace = inputImage.Convert<Gray, byte>())
            {
                return thumbnailGrayFace.Resize(_faceSize.Width, _faceSize.Height, INTER.CV_INTER_CUBIC);
            }
        }

        private void LoadTrainingImages()
        {
            var trainingSet = _unitOfWork.Repository<FaceDetection.Library.Models.Image>()
                .Query()
                .Include(img => img.Pictures.Select(picture => picture.Tags.Select(tag => tag.PersonOnImageTag))).Select().ToList();

            var inputIplImages = new List<Image<Gray, byte>>(trainingSet.Count);
            var personsOnImages = new List<Person>(trainingSet.Count);
            var imageConverter = new ImageConverter();

            foreach (var trainingSetImage in trainingSet)
            {
                var grayScalePicture = trainingSetImage.Pictures.FirstOrDefault(picture => picture.Type == PictureType.GrayscaleThumbnail);
                //var originalPicture = trainingSetImage.Pictures.FirstOrDefault(picture => picture.Type == PictureType.Original);
                var tagOnGrayScalePicture = grayScalePicture.Tags.FirstOrDefault();


                if (grayScalePicture != null && tagOnGrayScalePicture != null)
                {
                    string grayScalePicturePath = _basePicturesPath;
                    if (!string.IsNullOrWhiteSpace(grayScalePicture.PicturePath))
                    {
                        if (grayScalePicture.PicturePath.Contains(@"/"))
                        {
                            grayScalePicturePath += grayScalePicture.PicturePath.Replace(@"\\", "/");
                        }
                        else
                        {
                            grayScalePicturePath = grayScalePicture.PicturePath;
                        }
                        if (File.Exists(grayScalePicturePath))
                        {
                            var grayScaleImage = new Image<Gray, byte>(grayScalePicturePath.Replace(@"\\", @"\"));

                            inputIplImages.Add(grayScaleImage);
                            personsOnImages.Add(tagOnGrayScalePicture.PersonOnImageTag);
                        }
                    }
                    //var originalImage = BitmapConverter.ToIplImage(new Bitmap(new MemoryStream(originalPicture.PictureData)));
                    //var grayFromOriginalImage = CropAndNormalizeFace(originalImage, new Face(new Rectangle(tagOnGrayScalePicture.X, tagOnGrayScalePicture.Y, tagOnGrayScalePicture.Width, tagOnGrayScalePicture.Height)));

                }
            }

            _trainingImages = inputIplImages.ToArray();
            _personsOnImages = personsOnImages.ToArray();
        }

        private void LoadRecognizer()
        {
            switch (_faceRecognitionType)
            {
                case FaceRecognitionType.LBPHFaceRecognizer:
                    _faceRecognizer = new LBPHFaceRecognizer(1, 8, 8, 8, 100);
                    break;
                case FaceRecognitionType.FisherFaceRecognizer:
                    _faceRecognizer = new FisherFaceRecognizer(0, 3500);
                    break;
                case FaceRecognitionType.EigenFaceRecognizer:
                    _faceRecognizer = new EigenFaceRecognizer(80, double.PositiveInfinity);
                    break;
            }          
        }

        private void TrainRecognizer()
        {
            _faceRecognizer.Train(_trainingImages, _personsOnImages.Select(person => person.Id).ToArray());
            _isFaceRecognizerTrained = true;
        }

        private PersonWithFace RecognizeFace(Image<Gray, byte> faceImage)
        {
            var recognizerPrediction = _faceRecognizer.Predict(faceImage);
            int personId = 0;

            if (recognizerPrediction.Label == -1)
            {
                return null;
            }

            switch (_faceRecognitionType)
            {
                case FaceRecognitionType.LBPHFaceRecognizer:
                case FaceRecognitionType.FisherFaceRecognizer:
                    personId = recognizerPrediction.Label;
                    break;
                case FaceRecognitionType.EigenFaceRecognizer:
                    if (recognizerPrediction.Distance < _eigenDistanceThreshold)
                    {
                        personId = recognizerPrediction.Label;                    
                    }
                    else
                    {
                        var presumablyRecognizedPerson = _personsOnImages.FirstOrDefault(person => person.Id == recognizerPrediction.Label);

                        if (presumablyRecognizedPerson != null)
                        {
                            var presumablyRecognizedPersonWithFace = presumablyRecognizedPerson.ToPersonWithFace();
                            presumablyRecognizedPersonWithFace.EigenDistance = recognizerPrediction.Distance;
                            presumablyRecognizedPersonWithFace.RecognitionStatus = FaceRecognitionStatus.FaceRecognitionFailure;

                            return presumablyRecognizedPersonWithFace;
                        }
                        else
                        {
                            return new PersonWithFace
                            {
                                RecognitionStatus = FaceRecognitionStatus.FaceRecognitionFailure,
                                EigenDistance = recognizerPrediction.Distance
                            };
                        }                    
                    }
                    break;
            }

            var recognizedPerson = _personsOnImages.FirstOrDefault(person => person.Id == personId);

            if (recognizedPerson == null)
            {
                return new PersonWithFace
                {
                    EigenDistance = recognizerPrediction.Distance,
                    RecognitionStatus = FaceRecognitionStatus.FaceRecognitionFailure
                };
            }

            var personWithFace = recognizedPerson.ToPersonWithFace();
            personWithFace.EigenDistance = recognizerPrediction.Distance;
            personWithFace.RecognitionStatus = FaceRecognitionStatus.FaceRecognitionSuccessful;

            return personWithFace;
        }
    }
}
