using System;

namespace CV.Ads_Client.Utils
{
    public static class Logger
    {
        public static void StartNewSection()
        {
            Console.WriteLine("");
        }

        public static void Log(string messageSource, string message, ConsoleColor backgroundColor)
        {
            Console.ForegroundColor = backgroundColor;
            Console.WriteLine($"[{messageSource}] {message}");
            Console.ResetColor();
        }
    }
}
