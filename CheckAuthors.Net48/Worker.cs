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
#endregion

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CheckAuthors.Net48
{
    // usage: await new Worker().DoWorkAsync(path, mask)
    // https://wkalmar.github.io/post/batch-processing-with-enumeratefiles/
    public class Worker
    {
        //public const string Path = @"path to 200k files";
        private readonly FileProcessingService _processingService;

        public Worker()
        {
            _processingService = new FileProcessingService();
        }

        private string GetProperties(string file)
        {
            //using (var md5Instance = MD5.Create())
            //{
            //    using (var stream = File.OpenRead(file))
            //    {
            //        var hashResult = md5Instance.ComputeHash(stream);
            //        return BitConverter.ToString(hashResult)
            //            .Replace("-", "", StringComparison.OrdinalIgnoreCase)
            //            .ToLowerInvariant();
            //    }
            //}

            return "-";
        }

        private FileProcessingDto MapToDto(string file)
        {
            var fi = new FileInfo(file);

            return new FileProcessingDto();// (fi.Name, fi.Extension, fi.FullName, fi.DirectoryName, GetProperties(file));
        }

        public Task DoWork(string path, string mask) //v1
        {
            var files = Directory.GetFiles(path, mask, SearchOption.AllDirectories)
                .Select(p => MapToDto(p))
                .ToList();

            return _processingService.Process(files);
        }

        public async Task DoWorkAsync(string path, string mask) //v2
        {
            const int batchSize = 100; //10000
            var files = Directory.EnumerateFiles(path, mask, SearchOption.AllDirectories);
            var count = 0;
            var filesToProcess = new List<FileProcessingDto>(batchSize);

            foreach (var file in files)
            {
                count++;
                filesToProcess.Add(MapToDto(file));

                if (count == batchSize)
                {
                    await _processingService.Process(filesToProcess);
                    count = 0;
                    filesToProcess.Clear();
                }
            }

            if (filesToProcess.Any())
            {
                await _processingService.Process(filesToProcess);
            }
        }
    }
}
