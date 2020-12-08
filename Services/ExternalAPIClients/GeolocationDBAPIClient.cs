using CV.Ads_Client.Configuration.Model.Sections;
using CV.Ads_Client.Domain.ExternalAPIDTOs.GeolocationDBAPI;
using CV.Ads_Client.Utils;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace CV.Ads_Client.Services.ExternalAPIClients
{
    public class GeolocationDBAPIClient : IDisposable
    {
        private readonly HttpClient httpClient;
        private readonly GeolocationDBAPIConfigurationSection geolocationDBAPIConfiguration;

        public GeolocationDBAPIClient(GeolocationDBAPIConfigurationSection geolocationDBAPIConfiguration)
        {
            this.geolocationDBAPIConfiguration = geolocationDBAPIConfiguration;
            httpClient = new HttpClient();
        }

        public async Task<GeolocationResponse> RetreiveLocationAsync()
        {
            using var response = await httpClient.GetAsync(geolocationDBAPIConfiguration.RetreiveLocationURL);
            response.EnsureSuccessStatusCode();

            var location = await response.ReadResponseAsync<GeolocationResponse>(new SnakeCaseNamingPolicy());
            Logger.Log("geo", $"Location was retreived:\n\t{location}", ConsoleColor.White);

            return location;
        }

        public void Dispose() => httpClient.Dispose();
    }
}
