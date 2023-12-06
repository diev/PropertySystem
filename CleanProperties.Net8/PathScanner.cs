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

using System.Runtime.InteropServices;

using Microsoft.Extensions.Configuration;

namespace CleanProperties.Net8;

public class PathScanner
{
    private readonly EnumerationOptions _dirOptions = new(); // { RecurseSubdirectories = Settings.RecurseSubdirectories };
    private readonly EnumerationOptions _fileOptions = new();

    public PathScannerSettings Settings { get; set; } = new();

    public PathScanner(IConfiguration config)
    {
        config.Bind(nameof(PathScanner), Settings);

        if (!Settings.SkipHiddenDirs)
        {
            _dirOptions.AttributesToSkip = default(FileAttributes) | FileAttributes.System;
        }

        if (Settings.SkipReadOnlyFiles)
        {
            _fileOptions.AttributesToSkip |= FileAttributes.ReadOnly;
        }
    }

    public IEnumerable<string> EnumerateDirectories(string root)
    {
        if (Settings.DebugLog)
        {
            Logger.WriteNote(root, "...");
        }

        yield return root;

        foreach (var dir in Directory.EnumerateDirectories(root, "*", _dirOptions))
        {
            string newDir = dir;
            string name = Path.GetFileName(dir);

            if (name.StartsWith(' ') || name.EndsWith(' ')) // bug of NET
            {
                Logger.WriteNote(dir, "Error: Dir with spaces!");

                newDir = RemoveSideWhiteSpaces(dir);

                if (Directory.Exists(newDir))
                {
                    Logger.WriteNote(newDir, "Error: Dir exists!");
                    //goto SkipThisDir;

                    int i = 1;
                    while (Directory.Exists($"{newDir} ({++i})")) ;

                    newDir += $" ({i})";
                }

                if (RenameDirectoryWithSideWhiteSpaces(dir, newDir))
                {
                    Logger.WriteNote(newDir, "Error: Dir rename new!");
                }
                else
                {
                    Logger.WriteNote(newDir, "Error: Dir rename fails!");
                    goto SkipThisDir;
                }
            }

            foreach (var skip in Settings.SkipDirs)
            {
                if (newDir.StartsWith(skip, StringComparison.InvariantCultureIgnoreCase))
                {
                    goto SkipThisDir;
                }
            }

        SkipThisDir:
            
            foreach (var subdir in EnumerateDirectories(dir))
            {
                yield return subdir;
            }
        }
    }

    public IEnumerable<FileInfo> EnumerateFiles()
    {
        foreach (var root in Settings.ScanDirs)
        {
            foreach (var dir in EnumerateDirectories(Path.GetFullPath(root)))
            {
                foreach (var mask in Settings.FileMasks)
                {
                    foreach (var file in Directory.EnumerateFiles(dir, mask, _fileOptions))
                    {
                        yield return new FileInfo(file);
                    }
                }
            }
        }
    }

    // https://social.msdn.microsoft.com/Forums/en-US/92f8813f-5dbf-4e67-b8eb-2c91de2a6696/directorynotfoundexception-when-directory-name-ends-with-white-space?forum=csharpgeneral

    [DllImport("kernel32.dll", EntryPoint = "MoveFileW", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
    private static extern bool MoveFile(string lpExistingFileName, string lpNewFileName);

    private static bool RenameDirectoryWithSideWhiteSpaces(string path, string newPath)
    {
        var oldPath = @"\\?\" + path;
        var result = MoveFile(oldPath, newPath);

        return result;
    }

    private static string RemoveSideWhiteSpaces(string path)
    {
        //var parent = Path.GetDirectoryName(path);
        //var name = Path.GetFileName(path);
        //var result = Path.Combine(parent ?? Path.GetPathRoot(path), name.Trim());

        var breadcrumbs = path.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).Select(x => x.Trim()).ToArray();
        var result = string.Join(Path.DirectorySeparatorChar, breadcrumbs);

        return result;
    }
}
