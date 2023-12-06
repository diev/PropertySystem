#region License
/*
Copyright (c) Dmitrii Evdokimov
Source https://github.com/diev/

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

using System;
using System.Configuration;
using System.IO;
using System.Linq;

namespace CleanAuthors.Net48
{
    internal static class AppSettings
    {
        public static bool GetBoolean(string key)
        {
            string value = ConfigurationManager.AppSettings[key];
            return value.Equals("1") ||
                value.Equals("true", StringComparison.OrdinalIgnoreCase) ||
                value.Equals("on", StringComparison.OrdinalIgnoreCase);
        }

        public static string GetString(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public static string[] GetMultiString(string key)
        {
            string value = ConfigurationManager.AppSettings[key];
            return value.Split(';').Select(n => n.Trim()).ToArray();
        }

        public static string[] GetStrings(string key)
        {
            string value = ConfigurationManager.AppSettings[key];
            return value.Split('\n').Select(n => n.Trim()).Where(n => !string.IsNullOrEmpty(n)).ToArray();
        }

        public static string GetLogFileName(string key)
        {
            string value = ConfigurationManager.AppSettings[key];
            var now = DateTime.Now;
            string path = Directory.CreateDirectory(Path.Combine(value, $"{now:yyyy}")).FullName;
            return Path.Combine(path, $"{now:yyyyMMdd-HHmm}.log");
        }
    }
}
