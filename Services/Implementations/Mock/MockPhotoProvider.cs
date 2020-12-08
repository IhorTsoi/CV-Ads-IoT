using CV.Ads_Client.Services.Interfaces;
using CV.Ads_Client.Utils;
using System;
using System.Threading;

namespace CV.Ads_Client.Services.Implementations.Mock
{
    class MockPhotoProvider : IPhotoProvider
    {
        private readonly string[] samplePhotos = { 
            "resources/sample_photo_no_people.jpg",
            "resources/sample_photo.jpg" 
        };

        public string TakePhoto(string fileName)
        {
            Logger.Log("camera", "Taking the photo", ConsoleColor.White);
            Thread.Sleep(500);

            int randomSamplePhotoIndex = new Random().Next(samplePhotos.Length);
            return samplePhotos[randomSamplePhotoIndex];
        }
    }
}
