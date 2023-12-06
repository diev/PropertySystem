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

using System.Reflection;
using System.Text;

using Microsoft.Extensions.Configuration;

namespace CleanProperties.Net8;

public class Program
{
    private const string _appSettings = "appsettings.json";

    //public ProgramSettings Settings { get; set; }

    static int Main(string[] args)
    {
        int result;

#if !DEBUG
        try
        {
#endif

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            string curDirectory = Directory.GetCurrentDirectory();
            string appDirectory = Path.GetDirectoryName(Environment.ProcessPath) ?? curDirectory;


            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(curDirectory)
                .AddJsonFile(Path.Combine(appDirectory, _appSettings), true, false)
                .AddJsonFile(_appSettings, true, false) //CurrentDirectory
                .Build();

            Logger.Reset(config);

            result = Worker.DoAction(args, config);

#if !DEBUG
        }
        catch (Exception e)
        {
            Logger.Settings.LogToConsole = true;
            Logger.WriteLine("Exception: " + e.Message);

            if (e.InnerException != null)
            {
                Logger.WriteLine("Inner exception: " + e.InnerException.Message);
            }

            result = 1;
        }

        //if (Debugger.IsAttached)
        //{
        //    Console.WriteLine();
        //    Console.Write("Press any key to exit...");
        //    Console.ReadKey(true);
        //}
#endif

        return result;
    }

    public static void Usage()
    {
        var assembly = Assembly.GetExecutingAssembly();
        string app = Path.GetFileNameWithoutExtension(Environment.ProcessPath);

        string info = @$"{app} v{assembly.GetName().Version.ToString(3)}
{assembly.GetCustomAttributes<AssemblyDescriptionAttribute>().FirstOrDefault()?.Description}

Usage: {app} OPTIONS Filename

OPTIONS:
  -?                    Help.
  -enum [Filter]        Enumerate all properties optionally filtered
                        (starting with) by the 'Filter' value.
  -get  Property        Get the value for the 'Property' defined
                        by its Canonical Name.
  -set  Property Value  Set the 'Value' for the 'Property'.
  -info Property        Get a schema information on the 'Property'.

'Filename' may be a 'Directory' also (recursively):
  -check                Check properties for possible personal data.
  -clean                Clean properties with possible personal data.

  -batch                Process by {_appSettings}, not Filename.

Examples:
  {app} -enum foo.docx
  {app} -enum System.Photo foo.jpg
  {app} -get System.Author foo.jpg
  {app} -set System.Author ""Jane Smith;John Smith"" foo.docx
  {app} -set System.Photo.MeteringMode 2 foo.jpg
  {app} -set System.Photo.DateTaken ""4/27/2023 12:03:02"" foo.jpg
  {app} -check C:\TEMP
  {app} -clean foo.docx
  {app} -info System.Author

OS:        {Environment.OSVersion}
NET:       {Environment.Version}
Settings:  ""{Path.Combine(Directory.GetCurrentDirectory(), _appSettings)}""
Log:       ""{Logger.Settings.FileName}""

Press any key to exit...";

        Console.WriteLine(info);
        Console.ReadKey(true);
    }
}
