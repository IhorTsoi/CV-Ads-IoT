namespace CV.Ads_Client.Configuration.Model.Sections
{
    public class CVAdsAPIConfigurationSection
    {
        public string BaseURL { private get; set; }
        public string LoginRelativeURL { private get; set; }
        public string FaceDetectionRelativeURL { private get; set; }
        public string AdvertisementByEnvironmentRelativeURL { private get; set; }

        public string LoginURL => BaseURL + LoginRelativeURL;
        public string FaceDetectionURL => BaseURL + FaceDetectionRelativeURL;
        public string AdvertisementByEnvironmentURL => BaseURL + AdvertisementByEnvironmentRelativeURL;

        public string GetFullResourceURL(string relativeURL) => BaseURL + relativeURL;
    }
}
