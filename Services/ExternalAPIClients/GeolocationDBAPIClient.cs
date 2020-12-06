using CV.Ads_Client.Configuration.Model.Sections;
using CV.Ads_Client.Domain.ExternalAPIDTOs.GeolocationDBAPI;
using CV.Ads_Client.Utils;
using System.Net.Http;
using System.Threading.Tasks;

namespace CV.Ads_Client.Services.ExternalAPIClients
{
    class GeolocationDBAPIClient
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
            var response = await httpClient.GetAsync(geolocationDBAPIConfiguration.RetreiveLocationURL);
            response.EnsureSuccessStatusCode();

            var geolocationResponse = await response.ReadResponseAsync<GeolocationResponse>(new SnakeCaseNamingPolicy());
            return geolocationResponse;
        }
    }
}
