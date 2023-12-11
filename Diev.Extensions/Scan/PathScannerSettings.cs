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

namespace Diev.Extensions.Scan;

public sealed class PathScannerSettings
{
    // appsettings.json
    public string[] ScanDirs { get; set; } = []; // [Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)];
    public string[] SkipDirs { get; set; } = [];
    public string[] FileMasks { get; set; } = []; // ["*.doc*", "*.xls*"];
    public bool SkipHiddenDirs { get; set; } = false; // EnumerationOptions.AttributesToSkip
    public bool SkipReadOnlyFiles { get; set; } = false; // EnumerationOptions.AttributesToSkip
    public bool RecurseSubdirectories { get; set; } = false; // EnumerationOptions.RecurseSubdirectories
    public bool DebugLog { get; set; } = false;
}
