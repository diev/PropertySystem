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

using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

using Diev.Extensions.Http;
using Diev.Extensions.Smtp;

namespace Diev.Extensions.Settings;

public static class SettingsManager
{
    //private const string appsettings = "appsettings.json";
    //private const string usersettings = "usersettings.json";

    /// <summary>
    /// C:\ProgramData
    /// </summary>
    public static string ProgramData =>
        Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

    /// <summary>
    /// C:\Users\{UserName}\AppData\Local
    /// </summary>
    public static string UserData =>
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

    /// <summary>
    /// Return the path of User's settings
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="userProfile">true: C:\ProgramData or false: C:\Users\{UserName}\AppData\Local</param>
    /// <param name="addCompany">Company or nothing</param>
    /// <param name="addVersion">Version or nothing</param>
    /// <returns>C:\Users\{UserName}\AppData\Local\[Company]\{AppName}\[Version]\usersettings.json or
    /// C:\ProgramData\[Company]\{AppName}\[Version]\usersettings.json</returns>
    //public static string UserPath(string fileName = usersettings,
    //    bool userProfile = true, bool addCompany = true, bool addVersion = false)
    //{
    //    var assembly = Assembly.GetEntryAssembly();
    //    var assemblyName = assembly?.GetName();

    //    StringBuilder sb = new();

    //    // UserName
    //    string appData = Environment.GetFolderPath(userProfile
    //        ? Environment.SpecialFolder.LocalApplicationData // C:\Users\{UserName}\AppData\Local
    //        : Environment.SpecialFolder.CommonApplicationData); // C:\ProgramData

    //    sb.Append(appData).Append(Path.DirectorySeparatorChar);

    //    // Company
    //    var company = assembly?.GetCustomAttributes<AssemblyCompanyAttribute>()
    //        .FirstOrDefault()?.Company; // ?? "Company";

    //    if (addCompany && company != null)
    //    {
    //        sb.Append(company).Append(Path.DirectorySeparatorChar);
    //    }

    //    // AppName
    //    var app = assemblyName?.Name
    //        ?? Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0]);
    //    sb.Append(app).Append(Path.DirectorySeparatorChar);

    //    // Version
    //    var version = assemblyName?.Version?.ToString(); // ?? "1.0.0.0";

    //    if (addVersion && version != null)
    //    {
    //        sb.Append(version).Append(Path.DirectorySeparatorChar);
    //    }

    //    sb.Append(fileName);

    //    //return Path.Combine(appData,
    //    //    //company,
    //    //    app,
    //    //    //version, 
    //    //    fileName);

    //    return sb.ToString();
    //}

    public static string GetPassword(string? password)
    {
        if (password == null)
        {
            throw new ArgumentNullException(nameof(password), "No password configured.");
        }

        if (password.StartsWith("base64,", StringComparison.InvariantCultureIgnoreCase))
        {
            string value = password[7..]
                .ReplaceLineEndings()
                .Replace(" ", "")
                .Trim();
            string decoded = Encoding.UTF8.GetString(Convert.FromBase64String(value));

            return decoded;
        }

        return password;
    }
}
