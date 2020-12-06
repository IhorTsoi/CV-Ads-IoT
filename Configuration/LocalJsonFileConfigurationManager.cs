using CV.Ads_Client.Configuration.Model;
using System;
using System.IO;
using System.Text.Json;

namespace CV.Ads_Client.Configuration
{
    class LocalJsonFileConfigurationManager: IConfigurationManager
    {
        private readonly string _congigurationFilePath = "appsettings.json";

        public LocalJsonFileConfigurationManager()
        { }

        public LocalJsonFileConfigurationManager(string congigurationFilePath)
        {
            if (!File.Exists(congigurationFilePath))
            {
                throw new ArgumentException($"The file '{congigurationFilePath}' doesn't exist.");
            }
            _congigurationFilePath = congigurationFilePath;
        }

        public T RetreiveConfiguration<T>(Func<ConfigurationModel, T> retreiveFunction)
        {
            ConfigurationModel configuration = RetreiveConfigurationModel();
            return retreiveFunction(configuration);
        }

        public void UpdateConfiguration(Action<ConfigurationModel> updateFunction)
        {
            ConfigurationModel configuration = RetreiveConfigurationModel();
            updateFunction(configuration);

            string configurationUpdatedJson = JsonSerializer.Serialize(configuration, GetJsonOptions());
            File.WriteAllText(_congigurationFilePath, configurationUpdatedJson);
        }

        private ConfigurationModel RetreiveConfigurationModel()
        {
            string configurationJson = File.ReadAllText(_congigurationFilePath);
            var configuration = JsonSerializer.Deserialize<ConfigurationModel>(configurationJson, GetJsonOptions());

            return configuration;
        }

        private JsonSerializerOptions GetJsonOptions() =>
            new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    }
}
