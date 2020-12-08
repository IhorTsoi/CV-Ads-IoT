using CV.Ads_Client.Services.Interfaces;
using CV.Ads_Client.Utils;
using System;
using System.Threading;

namespace CV.Ads_Client.Services.Implementations.Mock
{
    class MockImageDisplayer : IImageDisplayer
    {
        public void Display(string imagePath, int durationInSeconds)
        {
            Logger.Log("display", $"Displaying the [{imagePath}]", ConsoleColor.White);
            Thread.Sleep(durationInSeconds * 1000);
            Logger.Log("display", $"Stop displaying the [{imagePath}]", ConsoleColor.White);
        }
    }
}
