using CV.Ads_Client.Configuration;
using CV.Ads_Client.Configuration.Model.Sections;
using CV.Ads_Client.Domain;
using CV.Ads_Client.Services.Interfaces;

namespace CV.Ads_Client.Services.Implementations
{
    public class CredentialsService : ICredentialsService
    {
        private readonly IConfigurationManager configurationManager;
        private readonly ICipherService cipherService;

        public CredentialsService(IConfigurationManager configurationManager, ICipherService cipherService)
        {
            this.configurationManager = configurationManager;
            this.cipherService = cipherService;
        }

        public Credentials GetCredentials()
        {
            var encryptedCredentials = configurationManager.RetreiveConfiguration(config => config.IoTConfiguration);
            Credentials credentials = new Credentials()
            {
                Login = cipherService.DecodeString(encryptedCredentials.SerialNumberEncrypted),
                Password = cipherService.DecodeString(encryptedCredentials.PasswordEncrypted)
            };

            return credentials;
        }

        public void UpdateCredentials(Credentials credentials)
        {
            var updatedIoTConfiguration = new IoTConfigurationSection()
            {
                SerialNumberEncrypted = cipherService.EncodeString(credentials.Login),
                PasswordEncrypted = cipherService.EncodeString(credentials.Password)
            };
            configurationManager.UpdateConfiguration(config => config.IoTConfiguration = updatedIoTConfiguration);
        }
    }
}
