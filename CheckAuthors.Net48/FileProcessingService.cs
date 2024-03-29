﻿#region License
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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CheckAuthors.Net48
{
    public class FileProcessingService
    {
        public Task Process(IReadOnlyCollection<FileProcessingDto> files, CancellationToken cancellationToken = default)
        {
            files.Select(p =>
            {
                //Console.WriteLine($"Processing {p.Name}...");
                return p;
            });

            //return Task.Delay(TimeSpan.FromMilliseconds(20), cancellationToken);
            return Task.CompletedTask;
        }
    }
}
