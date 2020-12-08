using CV.Ads_Client.Domain.Constants;

namespace CV.Ads_Client.Domain.ExternalAPIDTOs.CVAdsDTOs
{
    public class FaceDetectedResponse
    {
        public Gender Gender { get; set; }
        public int? Age { get; set; }

        public override string ToString()
        {
            return $"({Gender} - {Age})";
        }
    }
}
