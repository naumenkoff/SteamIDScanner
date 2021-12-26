using System;
using System.IO;

namespace SteamIDScanner
{
    public static class Program
    {
        public static string SteamFolder = Path.Combine(@"C:\Program Files (x86)", "Steam");

        private static void Main()
        {
            IDFinder.ScanDirectories(SteamFolder);
            Console.WriteLine(
                $"Просканировано {IDFinder.TotalScannedFolders} папок, {IDFinder.TotalScannedFiles} файлов, обнаружено {IDFinder.SteamProfiles.Count} аккаунтов");
            Console.ReadKey();
        }
        // .dmp .vdf .txt .mdmp .acf .json
    }
}