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

namespace Diev.Extensions.Http;

public class HttpSettings
{
    //public string BaseAddress { get; set; }
    public string? API { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public bool UseProxy { get; set; }
    public string? ProxyAddress { get; set; }
    public string UploadPath { get; set; } = ".";
    public string DownloadPath { get; set; } = ".";
}
