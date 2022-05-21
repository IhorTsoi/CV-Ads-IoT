using CV.Ads_Client.Services.Interfaces;
using System;
using System.Diagnostics;
using System.IO;

namespace CV.Ads_Client.Services.Implementations
{
    public class PythonPhotoProvider : IPhotoProvider
    {
        private readonly string _pathToStore;

        public PythonPhotoProvider(string pathToStore = "resources")
        {
            if (!Directory.Exists(pathToStore))
            {
                throw new ArgumentException($"The folder '{pathToStore}' doesn't exist.");
            }
            _pathToStore = pathToStore;
        }

        public string TakePhoto(string fileName)
        {
            string fullPathToPhoto = $"{_pathToStore}/{fileName}";
            ProcessStartInfo processStartInfo = new ProcessStartInfo("python", $"photoProvider.py {fullPathToPhoto}")
            {
                RedirectStandardError = true
            };

            using var streamerProcess = Process.Start(processStartInfo);
            string errorMessage = streamerProcess.StandardError.ReadToEnd();
            streamerProcess.WaitForExit();

            if (streamerProcess.ExitCode != 0)
            {
                throw new Exception($"Error in {nameof(StreamerPhotoProvider.TakePhoto)}. Message:\n{errorMessage}.");
            }
            return fullPathToPhoto;
        }
    }
}