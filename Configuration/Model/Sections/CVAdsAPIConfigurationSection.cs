namespace CV.Ads_Client.Configuration.Model.Sections
{
    public class CVAdsAPIConfigurationSection
    {
        public string BaseURL { get; set; }
        public string HubRelativeURL { get; set; }
        public string LoginRelativeURL { get; set; }
        public string FaceDetectionRelativeURL { get; set; }
        public string AdvertisementByEnvironmentRelativeURL { get; set; }

        public string GetHubURL() => BaseURL + HubRelativeURL;
        public string GetLoginURL() => BaseURL + LoginRelativeURL;
        public string GetFaceDetectionURL() => BaseURL + FaceDetectionRelativeURL;
        public string GetAdvertisementByEnvironmentURL() => BaseURL + AdvertisementByEnvironmentRelativeURL;

        public string GetFullResourceURL(string relativeURL) => BaseURL + relativeURL;
    }
}
