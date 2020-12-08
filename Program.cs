using CV.Ads_Client.Configuration;
using CV.Ads_Client.Domain.Constants;
using CV.Ads_Client.Domain.ExternalAPIDTOs.CVAdsDTOs;
using CV.Ads_Client.Routines;
using CV.Ads_Client.Routines.Implementations;
using CV.Ads_Client.Services.Caching;
using CV.Ads_Client.Services.ExternalAPIClients;
using CV.Ads_Client.Services.Implementations;
using CV.Ads_Client.Services.Implementations.Mock;
using CV.Ads_Client.Services.Interfaces;
using CV.Ads_Client.Utils;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace CV.Ads_Client
{
    class Program
    {
        static FileCachingService fileCache;
        static CVAdsAPIClient cvAdsAPIClient;
        static GeolocationDBAPIClient geoLocationAPIClient;

        static LoginResponse smartDeviceState;

        static ManualResetEvent shutDownWasEmmited;
        static AutoResetEvent reloginWasEmmited;

        static async Task Main()
        {
            using var serviceProvider = GetServiceProvider();
            InitializeServices(serviceProvider);
            await LoginAsync(serviceProvider);

            ConfigureShutDownBehaviour();
            await ConnectToHubAsync(serviceProvider);

            while (!shutDownWasEmmited.WaitOne(0))
            {
                if (reloginWasEmmited.WaitOne(0))
                {
                    Logger.StartNewSection();
                    await LoginAsync(serviceProvider);
                }
                IRoutine routine = GetRoutine(serviceProvider);
                await routine.RunAsync();
            }

            DisposeResources();
        }

        static ServiceProvider GetServiceProvider()
        {
            var serviceCollection = new ServiceCollection();
#if DEBUG
            serviceCollection.AddSingleton<IImageDisplayer, MockImageDisplayer>();
            serviceCollection.AddSingleton<IPhotoProvider, MockPhotoProvider>();
#else
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                serviceCollection.AddSingleton<IImageDisplayer, NomacsImageDisplayer>();
                serviceCollection.AddSingleton<IPhotoProvider, MockPhotoProvider>();
            }
            else
            {
                serviceCollection.AddSingleton<IImageDisplayer, EoGImageDisplayer>();
                serviceCollection.AddSingleton<IPhotoProvider, StreamerPhotoProvider>();
            }
#endif
            serviceCollection.AddSingleton<ICipherService, CaesarCipherService>();

            serviceCollection.AddSingleton<IConfigurationManager, LocalJsonFileConfigurationManager>();
            serviceCollection.AddSingleton<ICredentialsService, CredentialsService>();

            return serviceCollection.BuildServiceProvider();
        }

        static void InitializeServices(ServiceProvider serviceProvider)
        {
            var configuration = serviceProvider.GetService<IConfigurationManager>()
                .RetreiveConfiguration(config => config);

            fileCache = new FileCachingService(configuration.CacheCapacity);
            cvAdsAPIClient = new CVAdsAPIClient(configuration.CVAdsAPIConfiguration);
            geoLocationAPIClient = new GeolocationDBAPIClient(configuration.GeolocationDBAPIConfiguration);
        }

        static async Task LoginAsync(ServiceProvider serviceProvider)
        {
            var credentials = serviceProvider.GetService<ICredentialsService>().GetCredentials();
            try
            {
                smartDeviceState = await cvAdsAPIClient.LoginAsync(credentials);
                Logger.Log("routine", $"Logged in successfully. {smartDeviceState}", ConsoleColor.Green);
            }
            catch (Exception)
            {
                Logger.Log("routine", "The login process failed", ConsoleColor.Red);
                throw;
            }
        }

        static void ConfigureShutDownBehaviour()
        {
            shutDownWasEmmited = new ManualResetEvent(false);
            Console.CancelKeyPress += (sender, eventData) =>
            {
                eventData.Cancel = true;
                shutDownWasEmmited.Set();
                Logger.Log("application", "Terminating", ConsoleColor.Red);
            };
        }

        static async Task ConnectToHubAsync(ServiceProvider serviceProvider)
        {
            reloginWasEmmited = new AutoResetEvent(false);
            HubConnection hubConnection = CreateHubConnection(serviceProvider);

            hubConnection.Closed += async (error) =>
            {
                Logger.Log("signalR hub", "The connection was closed", ConsoleColor.Red);

                await LoginAsync(serviceProvider);
                await hubConnection.StartAsync();
                Logger.Log("signalR hub", "Reconnected to hub successfully", ConsoleColor.Green);
            };

            hubConnection.On("Update", () =>
            {
                Logger.Log("signalR hub", "Received 'Update' message", ConsoleColor.Blue);
                reloginWasEmmited.Set();
            });

            hubConnection.On("Activate", (string newPassword) =>
            {
                Logger.Log(
                    "signalR hub", $"Received 'Activate' message with new password: '{newPassword}'", ConsoleColor.Blue);
                serviceProvider.GetService<ICredentialsService>().UpdatePassword(newPassword);
                Task.Delay(2000).GetAwaiter().GetResult();
                reloginWasEmmited.Set();
            });

            try
            {
                await hubConnection.StartAsync();
                Logger.Log("signalR hub", "Connected to hub successfully", ConsoleColor.Green);
            }
            catch (HttpRequestException)
            {
                Logger.Log("signalR hub", "Failed to establish connection", ConsoleColor.Red);
                throw;
            }
        }

        static HubConnection CreateHubConnection(ServiceProvider serviceProvider)
        {
            var hubURL = serviceProvider.GetService<IConfigurationManager>()
                .RetreiveConfiguration(configuration => configuration.CVAdsAPIConfiguration.GetHubURL());
            var hubConnection = new HubConnectionBuilder()
                .WithUrl(hubURL, options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult(smartDeviceState.AccessToken);
                })
                .Build();
            return hubConnection;
        }

        static IRoutine GetRoutine(IServiceProvider serviceProvider)
        {
            IConfigurationManager configurationManager = serviceProvider.GetService<IConfigurationManager>();

            IRoutine routine = null;
            switch (smartDeviceState.Mode)
            {
                case SmartDeviceMode.Inactive:
                    routine = new InactiveRoutine(configurationManager);
                    break;
                case SmartDeviceMode.Active:
                    if (smartDeviceState.IsTurnedOn)
                    {
                        routine = new ActiveRoutine(
                            serviceProvider, fileCache, cvAdsAPIClient, geoLocationAPIClient, smartDeviceState);
                    }
                    else
                    {
                        routine = new ActiveTurnedOffRoutine(configurationManager);
                    }
                    break;
                case SmartDeviceMode.Blocked:
                    routine = new BlockedRoutine(configurationManager);
                    break;
            }

            return routine;
        }

        static void DisposeResources()
        {
            IDisposable[] services = {
                fileCache, cvAdsAPIClient, geoLocationAPIClient, shutDownWasEmmited, reloginWasEmmited };
            foreach (var service in services)
            {
                service.Dispose();
            }
        }
    }
}
