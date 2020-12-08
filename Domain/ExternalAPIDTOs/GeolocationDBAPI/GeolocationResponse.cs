namespace CV.Ads_Client.Domain.ExternalAPIDTOs.GeolocationDBAPI
{
    public class GeolocationResponse
    {
        public string CountryName { get; set; }
        public string City { get; set; }

        public override string ToString()
        {
            return $"{CountryName} - {City}";
        }
    }
}
