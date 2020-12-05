using System;
using Services.Implementations;
using Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace CV.Ads_Client
{
    class Program
    {
        static void Main(string[] args)
        {
            using var serviceProvider = GetServiceProvider();
            IImageDisplayer imageDisplayer = serviceProvider.GetService<IImageDisplayer>();
            IPhotoProvider photoProvider = serviceProvider.GetService<IPhotoProvider>();

            string fileName = $"me.jpeg";
            int showDurationInSeconds = 2;
            
            string pathToPhoto = photoProvider.TakePhoto(fileName);
            imageDisplayer.Display(pathToPhoto, showDurationInSeconds);
            File.Delete(pathToPhoto);
        }

        static ServiceProvider GetServiceProvider()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddTransient<IImageDisplayer, EoGImageDisplayer>();
            serviceCollection.AddTransient<IPhotoProvider, StreamerPhotoProvider>();

            return serviceCollection.BuildServiceProvider();
        }
    }
}
