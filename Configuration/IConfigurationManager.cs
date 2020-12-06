using CV.Ads_Client.Configuration.Model;
using System;

namespace CV.Ads_Client.Configuration
{
    public interface IConfigurationManager
    {
        public T RetreiveConfiguration<T>(Func<ConfigurationModel, T> retreiveFunction);
        public void UpdateConfiguration(Action<ConfigurationModel> updateFunction);
    }
}
