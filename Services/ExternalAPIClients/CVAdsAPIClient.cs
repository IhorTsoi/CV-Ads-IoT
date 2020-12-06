using CV.Ads_Client.Configuration.Model.Sections;
using CV.Ads_Client.Domain;
using CV.Ads_Client.Domain.ExternalAPIDTOs.CVAdsDTOs;
using CV.Ads_Client.Extensions;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace CV.Ads_Client.Services.ExternalAPIClients
{
    class CVAdsAPIClient: IDisposable
    {
        private readonly CVAdsAPIConfigurationSection cvAdsAPIConfiguration;
        private readonly HttpClient httpClient;

        public CVAdsAPIClient(CVAdsAPIConfigurationSection cvAdsAPIConfiguration)
        {
            this.cvAdsAPIConfiguration = cvAdsAPIConfiguration;
            httpClient = new HttpClient();
        }

        public async Task<LoginResponse> LoginAsync(Credentials credentials)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, cvAdsAPIConfiguration.LoginURL)
                .AddJsonContent(credentials);

            using var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            
            var loginResponse = await response.ReadResponseAsync<LoginResponse>();
            return loginResponse;
        }

        public async Task<FaceDetectedResponse[]> DetectFacesAsync(FileStream photo, string accessToken) 
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, cvAdsAPIConfiguration.FaceDetectionURL)
                .AddFormFileContent(photo)
                .SetAuthorizationHeader(accessToken);

            using var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var facesResponse = await response.ReadResponseAsync<FaceDetectedResponse[]>();
            return facesResponse;
        }

        public async Task<AdvertisementResponse> GetAdvertisementByEnvironmentAsync(
            GetAdvertisementByEnvironmentRequest envRequest, string accessToken)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Post, cvAdsAPIConfiguration.AdvertisementByEnvironmentURL)
                .AddJsonContent(envRequest)
                .SetAuthorizationHeader(accessToken);

            using var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var advertisementResponse = await response.ReadResponseAsync<AdvertisementResponse>();
            return advertisementResponse;
        }

        public void Dispose() => httpClient.Dispose();
    }
}
