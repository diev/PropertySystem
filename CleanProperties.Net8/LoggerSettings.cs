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

using System.Text;

namespace CleanProperties.Net8;

public sealed class LoggerSettings
{
    private const int _defaultEncoding = 1251;

    // appsetting.json
    public string FileNameFormat { get; set; } = @"logs\{0:yyyy}\{0:yyyyMMdd-HHmm}.log";
    public int FileEncoding { get; set; } = _defaultEncoding;
    public string LineFormat { get; set; } = @"{0:HH:mm:ss} {1}";
    public bool LogToConsole { get; set; } = false;

    // internal use
    public string FileName { get; set; } = "Trace.log";
    public Encoding EncodingValue { get; set; } = Encoding.GetEncoding(_defaultEncoding);
}
