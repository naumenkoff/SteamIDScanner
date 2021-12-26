using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SteamIDScanner
{
    public static class IDFinder
    {
        public static List<string> SteamProfiles = new List<string>();
        public static int TotalScannedFolders { get; set; }
        public static int TotalScannedFiles { get; set; }
        public static string ActivePath { get; set; }


        public static void ScanDirectories(string path)
        {
            var pathFolders = Directory.GetDirectories(path);
            foreach (var folder in pathFolders)
            {
                TotalScannedFolders++;
                var directoryThread = new ThreadController(() => { ScanDirectories(folder); });
                directoryThread.Start();
            }

            var fileThread = new ThreadController(() => { ScanFiles(path); });
            fileThread.Start();
        }

        private static void ScanFiles(string path)
        {
            var pathFiles = Directory.GetFiles(path);
            foreach (var file in pathFiles)
            {
                try
                {
                    ActivePath = file;
                    if (Path.HasExtension(file))
                    {
                        var extension = Path.GetExtension(file);
                        if (extension == ".dmp" || extension == ".vdf" ||
                            extension == ".txt" || extension == ".mdmp" ||
                            extension == ".acf" || extension == ".json")
                        {
                            var fileContent = File.ReadAllText(file);
                            if (CheckForID(fileContent)) Console.WriteLine($"[FILE CONTENT] [{DateTime.Now}] " + file);
                            var matches = Regex.Matches(fileContent, "\\\"765(?<id>[0-9]*)\\\"").Cast<Match>()
                                .Select(x => "765" + x.Groups["id"].Value);
                            foreach (var match in matches)
                            {
                                if (SteamProfiles.Contains(match) || string.IsNullOrEmpty(match) || match == "765")
                                    continue;
                                SteamProfiles.Add($"[{match}] => {file}");
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    // ignored
                }

                TotalScannedFiles++;
            }
        }

        private static bool CheckForID(string text)
        {
            return text.Contains("113621430") || text.Contains("76561198073887158");
        }
    }
}