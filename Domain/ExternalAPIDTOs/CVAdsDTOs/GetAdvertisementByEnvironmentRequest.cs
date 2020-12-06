namespace CV.Ads_Client.Domain.ExternalAPIDTOs.CVAdsDTOs
{
    public class GetAdvertisementByEnvironmentRequest
    {
        public FaceDetectedResponse[] Faces { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public int TimeZoneOffset { get; set; }
    }
}
