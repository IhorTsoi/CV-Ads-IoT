using CV.Ads_Client.Configuration;
using CV.Ads_Client.Utils;
using System;
using System.Threading.Tasks;

namespace CV.Ads_Client.Routines
{
    public abstract class BaseNoopRoutine : IRoutine
    {
        private readonly IConfigurationManager configurationManager;

        protected BaseNoopRoutine(IConfigurationManager configurationManager)
        {
            this.configurationManager = configurationManager;
        }

        public async Task RunAsync()
        {
            Logger.StartNewSection();

            Logger.Log("routine", "Routine type: " + GetRoutineName(), ConsoleColor.Green);
            await SleepAsync();
        }

        protected abstract string GetRoutineName();

        private async Task SleepAsync()
        {
            Logger.Log("routine", "Sleeping", ConsoleColor.Green);

            int freezeDurationInSeconds = configurationManager
                .RetreiveConfiguration(configuration => configuration.ShowDuration);
            await Task.Delay(freezeDurationInSeconds * 1000);
        }
    }
}
