#region License
//------------------------------------------------------------------------------
// Copyright (c) Dmitrii Evdokimov
// Source https://github.com/diev/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//------------------------------------------------------------------------------
// https://stackoverflow.com/questions/7296956/how-to-list-all-sub-directories-in-a-directory
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CheckAuthors.Net48
{
    public static class CustomSearcher
    {
        public static List<string> GetDirectories(string path, string searchPattern = "*",
            SearchOption searchOption = SearchOption.AllDirectories)
        {
            if (searchOption == SearchOption.TopDirectoryOnly)
                return Directory.GetDirectories(path, searchPattern).ToList();

            var directories = new List<string>(GetDirectories(path, searchPattern));
            
            for (var i = 0; i < directories.Count; i++)
                directories.AddRange(GetDirectories(directories[i], searchPattern));
            
            return directories;
        }

        private static List<string> GetDirectories(string path, string searchPattern)
        {
            try
            {
                return Directory.GetDirectories(path, searchPattern).ToList();
            }
            catch (UnauthorizedAccessException)
            {
                return new List<string>();
            }
        }

        public static List<string> GetFiles(string path, string searchPattern = "*",
            SearchOption searchOption = SearchOption.AllDirectories)
        {
            return new List<string>(Directory.GetFiles(path, searchPattern, searchOption));
        }

        public static List<DirectoryInfo> GetDirectories(DirectoryInfo di, string searchPattern = "*",
            SearchOption searchOption = SearchOption.AllDirectories)
        {
            if (searchOption == SearchOption.TopDirectoryOnly)
                return new List<DirectoryInfo>(di.GetDirectories(searchPattern));

            var directories = new List<DirectoryInfo>(di.GetDirectories(searchPattern));

            for (var i = 0; i < directories.Count; i++)
                directories.AddRange(GetDirectories(directories[i], searchPattern));

            return directories;
        }

        private static List<DirectoryInfo> GetDirectories(DirectoryInfo di, string searchPattern)
        {
            try
            {
                return new List<DirectoryInfo>(di.GetDirectories(searchPattern));
            }
            catch (UnauthorizedAccessException)
            {
                return new List<DirectoryInfo>();
            }
        }

        public static List<FileInfo> GetFiles(DirectoryInfo di, string searchPattern = "*",
            SearchOption searchOption = SearchOption.AllDirectories)
        {
            return new List<FileInfo>(di.GetFiles(searchPattern, searchOption));
        }
    }
}
