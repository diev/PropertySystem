#region License
/*
Copyright 2022-2023 Dmitrii Evdokimov
Open source software

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
#endregion

using System.Diagnostics;

using Diev.Extensions.Log;
using Diev.Extensions.Scan;

using iluvadev.ConsoleProgressBar; // https://github.com/iluvadev/ConsoleProgressBar

using Microsoft.Extensions.Configuration;

using static CleanProperties.ShellPack;

using Path = System.IO.Path;

namespace CleanProperties;

public static class Worker
{
    private static readonly EnumerationOptions _options = new() { RecurseSubdirectories = true };

    public static int DoAction(string[] args, IConfiguration config)
    {
        string cmd = args.Length == 0 ? "-?" : args[0];

        if (cmd.Equals("-get", StringComparison.OrdinalIgnoreCase) && args.Length == 3)
        {
            string propertyName = args[1];
            string fileName = Path.GetFullPath(args[2]);
            DisplayPropertyValue(fileName, propertyName);
            return 0;
        }

        if (cmd.Equals("-set", StringComparison.OrdinalIgnoreCase) && args.Length == 4)
        {
            string propertyName = args[1];
            string value = args[2];
            string fileName = Path.GetFullPath(args[3]);
            SetPropertyValue(fileName, propertyName, value);
            return 0;
        }

        if (cmd.Equals("-enum", StringComparison.OrdinalIgnoreCase))
        {
            if (args.Length == 2)
            {
                string fileName = Path.GetFullPath(args[1]);
                EnumProperties(fileName);
                return 0;
            }
            else if (args.Length == 3)
            {
                string filter = args[1];
                string fileName = Path.GetFullPath(args[2]);
                EnumProperties(fileName, filter);
                return 0;
            }
        }

        if (cmd.Equals("-info", StringComparison.OrdinalIgnoreCase) && args.Length == 2)
        {
            string propertyName = args[1];
            ShowPropertyInfo(propertyName);
            return 0;
        }

        if (cmd.Equals("-check", StringComparison.OrdinalIgnoreCase) && args.Length == 2)
        {
            string path = Path.GetFullPath(args[1]);
            CheckCommand(path);
            return 0;
        }

        if (cmd.Equals("-clean", StringComparison.OrdinalIgnoreCase) && args.Length == 2)
        {
            string path = Path.GetFullPath(args[1]);
            CleanCommand(path);
            return 0;
        }

        if (cmd.Equals("-batch", StringComparison.OrdinalIgnoreCase) && args.Length == 1)
        {
            BatchCommand(config);
            return 0;
        }

        Program.Usage();
        return 2;
    }

    private static void BatchCommand(IConfiguration config)
    {
        // https://github.com/iluvadev/ConsoleProgressBar

        using var pb = new ProgressBar() { Maximum = null, Delay = 1000 }; // Delay default = 75

        //Setting "Processing Text" with context 
        pb.Text.Body.Processing.SetValue(pb => pb.ElementName);

        //Setting "Done Text" with context
        pb.Text.Body.Done.SetValue(pb => $"Scanned {pb.Value} in {pb.TimeProcessing.Seconds}s.");

        //Clear "Description Text"
        pb.Text.Description.Clear();

        var watch = Stopwatch.StartNew();
        Logger.Write("Start");

        CleanerSettings cleaner = new();
        config.Bind(nameof(cleaner), cleaner);

        var scanner = new PathScanner();
        config.Bind(nameof(PathScanner), scanner);

        int filesCounter = 0, cleanCounter = 0, errorsCounter = 0;

        foreach (var file in scanner.EnumerateFiles())
        {
            filesCounter++;
            pb.PerformStep(file.DirectoryName);
            var line = GetPersonalProperties(file.FullName, cleaner.HideCommonNames);

            if (line != null)
            {
                string? error = cleaner.Clean
                    ? CleanPersonalProperties(file, cleaner.IgnoreReadOnly)
                    : " CleanTest!";

                if (error == null)
                {
                    cleanCounter++;
                    line += " ok";
                }
                else
                {
                    errorsCounter++;
                    line += error;
                }

                Logger.WriteNote(file.FullName, line);
            }
            else if (cleaner.DebugLog)
            {
                Logger.WriteNote(file.FullName, "ok");
            }
        }

        Logger.Write("Stop");
        watch.Stop();

        Logger.AppendLine($"Execution Time: {watch.Elapsed}");
        Logger.AppendLine($"Files: {filesCounter}, Clean: {cleanCounter}, Errors: {errorsCounter}");
        Logger.MarkOK();
    }

    private static void CleanCommand(string path)
    {
        if (Helper.IsDirectory(path))
        {
            foreach (var file in new DirectoryInfo(path).EnumerateFiles("*", _options))
            {
                DisplayToConsole(file);
            }
        }
        else
        {
            DisplayToConsole(new FileInfo(path));
        }

        static void DisplayToConsole(FileInfo file)
        {
            var line = GetPersonalProperties(file.FullName);

            if (line != null)
            {
                string? error = CleanPersonalProperties(file);

                if (error != null)
                {
                    Console.WriteLine(line + error);
                }
                else
                {
                    Console.WriteLine(line);
                }
            }
        }
    }

    private static void CheckCommand(string path)
    {
        if (Helper.IsDirectory(path))
        {
            foreach (string fileName in Directory.EnumerateFiles(path, "*", _options))
            {
                DisplayToConsole(fileName);
            }
        }
        else
        {
            DisplayToConsole(path);
        }

        static void DisplayToConsole(string fileName)
        {
            var line = GetPersonalProperties(fileName);

            if (line != null)
            {
                Console.WriteLine(line);
            }
        }
    }
}
