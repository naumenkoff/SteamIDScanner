using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SteamIDScanner;

public static class IDFinder
{
    public static readonly List<string> SteamProfiles = new();

    private static string _activePath;
    public static int TotalScannedFolders { get; private set; }
    public static int TotalScannedFiles { get; private set; }
    public static int TotalFailedFiles { get; private set; }

    public static void ScanDirectories(string path)
    {
        var pathFolders = Directory.GetDirectories(path);
        foreach (var folder in pathFolders)
        {
            TotalScannedFolders++;
            ScanDirectories(folder);
        }

        ScanFiles(path);
    }

    private static void ScanFiles(string path)
    {
        var pathFiles = Directory.GetFiles(path);
        var tasks = pathFiles.Select(CheckFile).ToList();
        Task.WhenAll(tasks);
    }

    private static async Task CheckFile(string file)
    {
        await Task.Run(() =>
        {
            try
            {
                _activePath = file;
                if (Path.HasExtension(file))
                {
                    var extension = Path.GetExtension(file);
                    if (extension is ".dmp" or ".vdf" or ".txt" or ".mdmp" or ".acf" or ".json")
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

                TotalScannedFiles++;
            }
            catch (Exception)
            {
                TotalFailedFiles++;
            }

            Console.Title = $"Directories: {TotalScannedFolders} | Files: {TotalScannedFiles} | {_activePath}";
            Task.Delay(1);
        });
    }

    private static bool CheckForID(string text)
    {
        return text.Contains("113621430") || text.Contains("76561198073887158");
    }
}