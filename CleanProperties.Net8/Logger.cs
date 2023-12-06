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

using Microsoft.Extensions.Configuration;

using Path = System.IO.Path;

namespace CleanProperties.Net8;

public static class Logger
{
    private static readonly List<string> _lines = new();

    public static LoggerSettings Settings { get; set; } = new();

    public static void Reset(IConfiguration config)
    {
        _lines.Clear();
        config.Bind(nameof(Logger), Settings);

        Settings.FileName = Path.GetFullPath(string.Format(Environment.ExpandEnvironmentVariables(
            Settings.FileNameFormat), DateTime.Now));
        Directory.CreateDirectory(Path.GetDirectoryName(Settings.FileName)!);

        Settings.EncodingValue = Encoding.GetEncoding(Settings.FileEncoding);
    }

    public static void Append(string text)
    {
        string line = string.Format(Settings.LineFormat!, DateTime.Now, text);
        _lines.Add(line);
    }

    public static void AppendNote(string path, string text)
    {
        Append($"\"{path}\" {text}");
    }

    public static void AppendLine(string text)
    {
        _lines.Add(text);
    }

    public static void Write(string text)
    {
        string line = string.Format(Settings.LineFormat!, DateTime.Now, text);

        Flush();
        File.AppendAllText(Settings.FileName!, line + Environment.NewLine, Settings.EncodingValue!);

        if (Settings.LogToConsole)
        {
            Console.WriteLine(line);
        }
    }

    public static void WriteNote(string path, string text)
    {
        Write($"\"{path}\" {text}");
    }

    public static void WriteNote(string path, string properties, string text)
    {
        Write($"\"{path}\"{properties} {text}");
    }

    public static void WriteLine(string text)
    {
        Flush();
        File.AppendAllText(Settings.FileName, text + Environment.NewLine, Settings.EncodingValue);

        if (Settings.LogToConsole)
        {
            Console.WriteLine(text);
        }
    }

    public static void Flush()
    {
        if (_lines.Count > 0)
        {
            var lines = _lines.ToArray();
            File.AppendAllLines(Settings.FileName, lines, Settings.EncodingValue);

            if (Settings.LogToConsole)
            {
                Console.WriteLine(string.Join(Environment.NewLine, lines));
            }

            _lines.Clear();
        }
    }

    public static void MarkOK()
    {
        string file = Settings.FileName;
        string ext = Path.GetExtension(file);
        string newFile = Path.ChangeExtension(file, ".ok" + ext);

        Flush();

        try
        {
            File.Move(file, newFile, true);
            Settings.FileName = newFile;
        }
        catch 
        { 
            WriteLine($"Error: rename to \"{newFile}\" failed!");
        }
    }
}
