using CV.Ads_Client.Configuration.Model.Sections;
using CV.Ads_Client.Domain;
using CV.Ads_Client.Domain.ExternalAPIDTOs.CVAdsDTOs;
using CV.Ads_Client.Utils;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace CV.Ads_Client.Services.ExternalAPIClients
{
    public class CVAdsAPIClient : IDisposable
    {
        private readonly CVAdsAPIConfigurationSection cvAdsAPIConfiguration;
        private readonly HttpClient httpClient;

        public CVAdsAPIClient(CVAdsAPIConfigurationSection cvAdsAPIConfiguration)
        {
            this.cvAdsAPIConfiguration = cvAdsAPIConfiguration;
            httpClient = new HttpClient(new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = (_1, _2, _3, _4) => true
            });
        }

        public async Task<LoginResponse> LoginAsync(Credentials credentials)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, cvAdsAPIConfiguration.GetLoginURL())
                .AddJsonContent(credentials);

            using var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var loginResponse = await response.ReadResponseAsync<LoginResponse>();
            return loginResponse;
        }

        public async Task<FaceDetectedResponse[]> DetectFacesAsync(FileStream photo, string accessToken)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, cvAdsAPIConfiguration.GetFaceDetectionURL())
                .AddFormFileContent(photo)
                .SetAuthorizationHeader(accessToken);

            using var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var facesDetected = await response.ReadResponseAsync<FaceDetectedResponse[]>();
            LogFaceDetectionResult(facesDetected);
            return facesDetected;
        }

        public async Task<AdvertisementResponse> GetAdvertisementByEnvironmentAsync(
            GetAdvertisementByEnvironmentRequest envRequest, string accessToken)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Post, cvAdsAPIConfiguration.GetAdvertisementByEnvironmentURL())
                .AddJsonContent(envRequest)
                .SetAuthorizationHeader(accessToken);

            using var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return null;
            }
            var advertisementResponse = await response.ReadResponseAsync<AdvertisementResponse>();
            return advertisementResponse;
        }

        public async Task<string> DownloadAdvertisementPictureAsync(string pictureURL)
        {
            using var response = await httpClient.GetAsync(cvAdsAPIConfiguration.BaseURL + pictureURL);
            response.EnsureSuccessStatusCode();

            Logger.Log("api", "The advertisement picture was downloaded", ConsoleColor.White);

            var localPicturePath = "downloads/" + pictureURL.Split('/')[^1];
            return await response.SaveResponseFileAsync(localPicturePath);
        }

        public void Dispose() => httpClient.Dispose();

        private static void LogFaceDetectionResult(FaceDetectedResponse[] facesDetected)
        {
            if (facesDetected.Length == 0)
            {
                Logger.Log("face detection", "No faces were detected", ConsoleColor.White);
            }
            else
            {
                Logger.Log("face detection", $"Following faces were detected:\n\t" +
                    $"{string.Join(", ", facesDetected.AsEnumerable())}", ConsoleColor.White);
            }
        }
    }
}
