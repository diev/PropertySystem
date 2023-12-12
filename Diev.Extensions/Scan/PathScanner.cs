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

namespace Diev.Extensions.Scan;

public partial class PathScanner
{
    private readonly EnumerationOptions _dirOptions = new(); // { RecurseSubdirectories = Settings.RecurseSubdirectories };
    private readonly EnumerationOptions _fileOptions = new();

    /// <summary>
    /// Get the currently processed root directory.
    /// </summary>
    public string CurrentRoot { get; private set; } = Path.DirectorySeparatorChar.ToString();

    /// <summary>
    /// An array of root directory names to process.
    /// Process the current directory by default.
    /// </summary>
    public string[] ScanDirs { get; set; } = ["."]; // [Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)];

    /// <summary>
    /// An array of partial path directory names to skip inner areas.
    /// Empty by default.
    /// </summary>
    public string[] SkipDirs { get; set; } = [];

    /// <summary>
    /// An array of file name masks to process.
    /// Process "*" by default (not "*.*"!).
    /// </summary>
    public string[] FileMasks { get; set; } = ["*"]; // ["*.doc*", "*.xls*"];

    /// <summary>
    /// Skip if the hidden attribute is set.
    /// False by default.
    /// </summary>
    public bool SkipHidden { get; set; } = false; // EnumerationOptions.AttributesToSkip

    /// <summary>
    /// Skip if the readonly attribute is set.
    /// False by default.
    /// </summary>
    public bool SkipReadOnly { get; set; } = false; // EnumerationOptions.AttributesToSkip

    /// <summary>
    /// Process recursively.
    /// This directory only by default.
    /// </summary>
    public bool RecurseSubdirectories { get; set; } = false; // EnumerationOptions.RecurseSubdirectories

    /// <summary>
    /// Clean directory/file names from unwanted chars.
    /// Try to workaround .NET https://github.com/dotnet/runtime/issues/95867 also.
    /// </summary>
    public bool NormalizeNames { get; set; } = false;

    public PathScanner()
    {
    }

    public IEnumerable<string> EnumerateDirectories(string root)
    {
        Console.WriteLine($"root: {root}");
        yield return root;

        foreach (var dir in Directory.EnumerateDirectories(root, "*", _dirOptions))
        {
            string newDir = NormalizeNames ? NormalizeName(dir) : dir;

            foreach (var skip in SkipDirs)
            {
                if (newDir.StartsWith(skip, StringComparison.InvariantCultureIgnoreCase))
                {
                    Console.WriteLine($"skip: {skip}");
                    goto SkipThisDir;
                }
            }

            foreach (var subdir in EnumerateDirectories(newDir))
            {
                Console.WriteLine($"subdir: {subdir}");
                yield return subdir;
            }

        SkipThisDir:
            ;
        }
    }

    /// <summary>
    /// Enumerate all files selected by condition Properties.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<FileInfo> EnumerateFiles()
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

        foreach (var root in ScanDirs)
        {
            CurrentRoot = root;

            foreach (var dir in EnumerateDirectories(Path.GetFullPath(root)))
            {
                Console.WriteLine($"dir: {dir}");

                foreach (var mask in FileMasks)
                {
                    foreach (var file in Directory.EnumerateFiles(dir, mask, _fileOptions))
                    {
                        Console.WriteLine($"file: {file}");

                        yield return new FileInfo(NormalizeNames ? NormalizeName(file) : file);
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

    private static bool RenameWithSideSpaces(string path, string newPath)
    {
        return MoveFile(PrefixPath(path), newPath);
    }

    private static string PrefixPath(string path)
    {
        return path.StartsWith(@"\\?\", StringComparison.Ordinal) ? path : @"\\?\" + path;
    }

    private static string NextFreeName(string path, bool folder = false)
    {
        string name = Path.ChangeExtension(path, null); 
        string ext = Path.GetExtension(path);
        int i = 1;

        if (folder)
        {
            if (!Directory.Exists(path))
            {
                return path;
            }

            while (Directory.Exists($"{name} ({++i}){ext}"))
                ;
        }
        else
        { 
            if (!File.Exists(path))
            {
                return path;
            }

            while (File.Exists($"{name} ({++i}){ext}")) //TODO "Name (2) (2).txt" => "Name (3).txt"
                ;
        }

        return $"{name} ({i}){ext}";
    }

    private static string NormalizeName(string path, bool folder = false)
    {
        string name = Path.GetFileNameWithoutExtension(path);
        string ext = Path.GetExtension(path);

        if (path.EndsWith('.')) // a bug of .NET to lost the last dot if any
        {
            name += '.';
        }

        string name2 = name;
        string ext2 = ext;

        if (name2.Contains(' '))
        {
            name2 = name2.Trim();
            while (name2.Contains("  "))
                name2 = name2.Replace("  ", " ");
            while (name2.Contains(" ,"))
                name2 = name2.Replace(" ,", ",");
            while (name2.Contains("( "))
                name2 = name2.Replace("( ", "(");
            while (name2.Contains(" )"))
                name2 = name2.Replace(" )", ")");
        }

        if (name2.Contains('.'))
        {
            while (name2.Contains(".."))
                name2 = name2.Replace("..", ".");
            while (name2.EndsWith('.'))
                name2 = name2[..^1];
        }

        if (ext2.Contains(' '))
        {
            ext2 = ext2[1..].Trim();
            ext2 = '.' + string.Join(' ', ext2.Split(' ', StringSplitOptions.RemoveEmptyEntries));
        }

        name += ext;
        name2 += ext2;

        string path2 = NextFreeName(Path.Combine(Path.GetDirectoryName(path), name2), folder);

        if (!name.Equals(name2, StringComparison.OrdinalIgnoreCase))
        {
            if (!RenameWithSideSpaces(path, path2))
            {
                throw new FileNotFoundException("Path not operable.", path);
            }
        }

        Console.WriteLine($"norm: {path2}");
        return path2;
    }
}
