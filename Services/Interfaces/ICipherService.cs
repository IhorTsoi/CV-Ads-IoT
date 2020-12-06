namespace CV.Ads_Client.Services.Interfaces
{
    public interface ICipherService
    {
        public string EncodeString(string originalString);
        public string DecodeString(string encodedString);
    }
}
