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

namespace Diev.Extensions.Scan;

public partial class PathScanner
{
    private readonly EnumerationOptions _dirOptions = new(); // { RecurseSubdirectories = Settings.RecurseSubdirectories };
    private readonly EnumerationOptions _fileOptions = new();

    public string CurrentRoot { get; private set; } = Path.DirectorySeparatorChar.ToString();

    public string[] ScanDirs { get; set; } = []; // [Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)];
    public string[] SkipDirs { get; set; } = [];
    public string[] FileMasks { get; set; } = []; // ["*.doc*", "*.xls*"];
    public bool SkipHidden { get; set; } = false; // EnumerationOptions.AttributesToSkip
    public bool SkipReadOnly { get; set; } = false; // EnumerationOptions.AttributesToSkip
    public bool RecurseSubdirectories { get; set; } = false; // EnumerationOptions.RecurseSubdirectories

    public PathScanner()
    {
    }

    public void Reset()
    {
        if (!SkipHidden)
        {
            _dirOptions.AttributesToSkip = default(FileAttributes) | FileAttributes.System;
            _fileOptions.AttributesToSkip = default(FileAttributes) | FileAttributes.System;
        }

        if (SkipReadOnly)
        {
            _dirOptions.AttributesToSkip |= FileAttributes.ReadOnly;
            _fileOptions.AttributesToSkip |= FileAttributes.ReadOnly;
        }

        CurrentRoot = ScanDirs[0];
    }

    public static void TestMyDocuments() // Demo of usage
    {
        var scanner = new PathScanner()
        {
            ScanDirs = [Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)]
        };

        foreach (var file in scanner.EnumerateFiles())
        {
            Console.WriteLine(file.DirectoryName);
        }
    }

    public IEnumerable<string> EnumerateDirectories(string root)
    {
        yield return root;

        foreach (var dir in Directory.EnumerateDirectories(root, "*", _dirOptions))
        {
            string newDir = dir;
            string name = Path.GetFileName(dir);

            if (name.StartsWith(' ') || name.EndsWith(' ')) // bug of NET!
            {
                newDir = RemoveSideWhiteSpaces(dir);

                if (Directory.Exists(newDir))
                {
                    int i = 1;
                    while (Directory.Exists(newDir + $" ({++i})")) ;

                    newDir += $" ({i})";
                }

                if (!RenameDirectoryWithSideWhiteSpaces(dir, newDir))
                {
                    goto SkipThisDir;
                }
            }

            foreach (var skip in SkipDirs)
            {
                if (newDir.StartsWith(skip, StringComparison.InvariantCultureIgnoreCase))
                {
                    goto SkipThisDir;
                }
            }

            foreach (var subdir in EnumerateDirectories(newDir))
            {
                yield return subdir;
            }

        SkipThisDir:
            ;
        }
    }

    public IEnumerable<FileInfo> EnumerateFiles()
    {
        foreach (var root in ScanDirs)
        {
            CurrentRoot = root;

            foreach (var dir in EnumerateDirectories(Path.GetFullPath(root)))
            {
                foreach (var mask in FileMasks)
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

    [LibraryImport("kernel32.dll", EntryPoint = "MoveFileW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool MoveFile(string lpExistingFileName, string lpNewFileName);

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
