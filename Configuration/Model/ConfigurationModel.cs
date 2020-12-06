using CV.Ads_Client.Configuration.Model.Sections;

namespace CV.Ads_Client.Configuration.Model
{
    public class ConfigurationModel
    {
        public int ShowDuration { get; set; }
        public int CacheCapacity { get; set; }

        public IoTConfigurationSection IoTConfiguration { get; set; }
        public CVAdsAPIConfigurationSection CVAdsAPIConfiguration { get; set; }
        public GeolocationDBAPIConfigurationSection GeolocationDBAPIConfiguration { get; set; }
    }
}
