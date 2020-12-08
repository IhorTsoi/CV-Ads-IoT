using CV.Ads_Client.Configuration;
using CV.Ads_Client.Domain.ExternalAPIDTOs.CVAdsDTOs;
using CV.Ads_Client.Domain.ExternalAPIDTOs.GeolocationDBAPI;
using CV.Ads_Client.Services.Caching;
using CV.Ads_Client.Services.ExternalAPIClients;
using CV.Ads_Client.Services.Interfaces;
using CV.Ads_Client.Utils;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CV.Ads_Client.Routines
{
    public class ActiveRoutine : IRoutine
    {
        private readonly FileCachingService fileCache;
        private readonly CVAdsAPIClient cvAdsAPIClient;
        private readonly GeolocationDBAPIClient geoLocationAPIClient;

        private readonly LoginResponse smartDeviceState;

        private readonly IPhotoProvider photoProvider;
        private readonly IImageDisplayer imageDisplayer;
        private readonly IConfigurationManager configurationManager;

        public ActiveRoutine(
            ServiceProvider serviceProvider,
            FileCachingService fileCache,
            CVAdsAPIClient cvAdsAPIClient,
            GeolocationDBAPIClient geoLocationAPIClient,
            LoginResponse smartDeviceState)
        {
            this.fileCache = fileCache;
            this.cvAdsAPIClient = cvAdsAPIClient;
            this.geoLocationAPIClient = geoLocationAPIClient;
            this.smartDeviceState = smartDeviceState;

            photoProvider = serviceProvider.GetService<IPhotoProvider>();
            imageDisplayer = serviceProvider.GetService<IImageDisplayer>();
            configurationManager = serviceProvider.GetService<IConfigurationManager>();
        }

        public async Task RunAsync()
        {
            Logger.StartNewSection();

            var location = await geoLocationAPIClient.RetreiveLocationAsync();

            var photoPath = photoProvider.TakePhoto(CreateEnvironmentPhotoName());
            using var photo = File.OpenRead(photoPath);

            var facesDetected = await cvAdsAPIClient.DetectFacesAsync(photo, smartDeviceState.AccessToken);
            if (facesDetected.Length == 0)
            {
                await SleepAsync();
                return;
            }

            var environment = GetEnvironmentData(location, facesDetected);
            AdvertisementResponse advertisement = await cvAdsAPIClient
                .GetAdvertisementByEnvironmentAsync(environment, smartDeviceState.AccessToken);
            if (advertisement == null)
            {
                Logger.Log("routine", "No suitable advertisement was found", ConsoleColor.Green);
                await SleepAsync();
                return;
            }
            Logger.Log("routine", $"Advertisement was found: {advertisement.Name}", ConsoleColor.Green);
            
            string localPathToAdPicture = await LoadPicturePath(advertisement);
            int showDuration = configurationManager.RetreiveConfiguration(config => config.ShowDuration);
            imageDisplayer.Display(localPathToAdPicture, showDuration);
            if (!smartDeviceState.IsCaching)
            {
                File.Delete(localPathToAdPicture);
            }
        }

        private async Task<string> LoadPicturePath(AdvertisementResponse advertisement)
        {
            string localPathToAdPicture = null;
            if (smartDeviceState.IsCaching)
            {
                localPathToAdPicture = fileCache.Get(advertisement.Url);
            }
            
            if (localPathToAdPicture == null)
            {
                localPathToAdPicture = await cvAdsAPIClient
                    .DownloadAdvertisementPictureAsync(advertisement.Url);
                if (smartDeviceState.IsCaching)
                {
                    fileCache.Add(advertisement.Url, localPathToAdPicture);
                }
            }

            return localPathToAdPicture;
        }

        private async Task SleepAsync()
        {
            Logger.Log("routine", "Sleeping", ConsoleColor.Green);

            int freezeDurationInSeconds = configurationManager
                .RetreiveConfiguration(configuration => configuration.ShowDuration);
            await Task.Delay(freezeDurationInSeconds * 1000);
        }

        private static string CreateEnvironmentPhotoName() => $"{Guid.NewGuid()}.jpg";

        private static GetAdvertisementByEnvironmentRequest GetEnvironmentData(
            GeolocationResponse location, FaceDetectedResponse[] faceDetected) =>
            new GetAdvertisementByEnvironmentRequest()
            {
                Faces = faceDetected,
                Country = location.CountryName,
                City = location.City,
                TimeZoneOffset = DateTimeOffset.Now.Offset.Hours
            };
    }
}
