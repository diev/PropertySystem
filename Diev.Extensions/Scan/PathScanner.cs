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
using System.Security.Cryptography;
using System.Text;

using Diev.Extensions.Log;

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
    public string[] ScanDirs { get; set; } = []; // [Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)];

    /// <summary>
    /// An array of partial path directory names to skip inner areas.
    /// Empty by default.
    /// </summary>
    public string[] SkipDirs { get; set; } = [];

    /// <summary>
    /// An array of file name masks to process.
    /// Process "*" by default (not "*.*"!).
    /// </summary>
    public string[] FileMasks { get; set; } = []; // ["*.doc*", "*.xls*"];

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
        yield return root;

        foreach (var dir in Directory.EnumerateDirectories(root, "*", _dirOptions))
        {
            string newDir = NormalizeNames ? NormalizeName(dir) : dir;

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

        if (ScanDirs.Length == 0)
        {
            ScanDirs = ["."];
        }

        if (FileMasks.Length == 0)
        {
            FileMasks = ["*"];
        }

        foreach (var root in ScanDirs)
        {
            CurrentRoot = root;

            foreach (var dir in EnumerateDirectories(Path.GetFullPath(root)))
            {
                foreach (var mask in FileMasks)
                {
                    foreach (var file in Directory.EnumerateFiles(dir, mask, _fileOptions))
                    {
                        string path = file;

                        if (NormalizeNames)
                        {
                            try
                            {
                                path = NormalizeName(file);
                            }
                            catch (Exception e)
                            {
                                Logger.LastError(e);
                                continue;
                            }
                        }

                        if (File.Exists(path)) // wonderful but real!
                        {
                            yield return new FileInfo(path);
                        }
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

    /// <summary>
    /// Creates a new free file name to rename the source file if its content differs.
    /// </summary>
    /// <param name="path">Source filename.</param>
    /// <param name="tryPath">Proposed filename to check.</param>
    /// <param name="newPath">Destination filename to rename.</param>
    /// <returns>Rename required (no same files by content found).</returns>
    private static bool TryNextFreeName(string path, string tryPath, out string newPath)
    {
        newPath = tryPath;

        if (!File.Exists(tryPath))
        {
            // proposed filename is free - rename to it correctly
            return true;
        }

        var hash = GetFileHash(path);
        var tryHash = GetFileHash(tryPath);

        if (tryHash.SequenceEqual(hash)) // tryHash.Equals(hash) and == do not work!
        {
            // proposed filename has same content - delete bad source, rename not required
            File.Delete(path);

            return false;
        }

        string name = Path.ChangeExtension(tryPath, null);
        string ext = Path.GetExtension(tryPath);
        int i = 1;

        newPath = $"{name} ({++i}){ext}";

        while (File.Exists(newPath)) //TODO "Name (2) (2).txt" => "Name (3).txt"
        {
            var newHash = GetFileHash(newPath);

            if (newHash.SequenceEqual(hash))
            {
                // calculated filename has same content - delete bad source, rename not required
                File.Delete(path);

                return false;
            }

            newPath = $"{name} ({++i}){ext}";
        }

        // rename required
        return true;
    }

    /// <summary>
    /// Get a new normalized directory/file name.
    /// </summary>
    /// <param name="path">Directory/file name.</param>
    /// <param name="folder">True if directory, false if file (default).</param>
    /// <returns>Normalized name.</returns>
    /// <exception cref="FileNotFoundException"></exception>
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
            ext2 = string.Join(' ', ext2.TrimEnd()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries));
        }

        name += ext;
        name2 += ext2;

        if (!name.Equals(name2, StringComparison.OrdinalIgnoreCase))
        {
            // names are different - rename required

            name2 = Path.Combine(Path.GetDirectoryName(path), name2);

            if (folder)
            {
                string path2 = NextFreeName(name2, folder); //TODO try to find same ready directory (by comparing all trees and all contents?)

                if (!RenameWithSideSpaces(path, path2))
                {
                    throw new FileNotFoundException("Directory path not operable.", path);
                }

                // new folder name
                return path2;
            }
            else if (TryNextFreeName(path, name2, out string path2)) // try to find same ready file
            {
                if (!RenameWithSideSpaces(path, path2))
                {
                    throw new FileNotFoundException("File path not operable.", path);
                }

                // not found - return new filename
                return path2;
            }
            else // found same ready file
            {
                // return good named existing filename (bad source is deleted)
                return path2;
            }
        }

        // return as is - no rename required
        return path;
    }

    /// <summary>
    /// Calculates the hash of a file to compare later.
    /// </summary>
    /// <param name="file1">File to calc.</param>
    /// <returns>Comparable string value of hash.</returns>
    private static byte[] GetFileHash(string file)
    {
        using var hasher = SHA1.Create(); // MD5.Create()
        using var stream = File.OpenRead(PrefixPath(file));
        var hash = hasher.ComputeHash(stream);

        return hash;
    }
}
