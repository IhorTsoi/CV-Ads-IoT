using CV.Ads_Client.Domain;

namespace CV.Ads_Client.Services.Interfaces
{
    public interface ICredentialsService
    {
        public Credentials GetCredentials();
        public void UpdateCredentials(Credentials credentials);
    }
}
