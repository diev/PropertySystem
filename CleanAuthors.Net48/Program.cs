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

using Microsoft.WindowsAPICodePack.Shell;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

using Task = System.Threading.Tasks.Task;

namespace CleanAuthors.Net48
{
    internal class Program
    {
        //app.config
        private static readonly string _path = GetString("Path");
        private static readonly string[] _skips = GetStrings("Skip");
        private static readonly string[] _masks = GetMultiString("Masks");
        private static readonly bool _subdirs = GetBoolean("Subdirs");
        private static readonly bool _readonly = GetBoolean("ReadOnly");
        private static readonly string _logFileName = GetLogFileName("Logs");

        //counters
        private static int _level = 0;
        private static int _totalLevels = 0;
        private static int _totalDirs = 0;
        private static int _totalFiles = 0;
        private static int _totalClean = 0;
        private static int _totalErrors = 0;

        private static readonly Queue<string> _log = new Queue<string>();

        private static async Task Main(string[] args)
        {
            try
            {
                using (var stream = new FileStream(_logFileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read, 4096, true))
                using (var writer = new StreamWriter(stream, Encoding.GetEncoding(1251)))
                {
                    bool run = true;
                    var logger = Task.Run(async () =>
                    {
                        while (run)
                        {
                            if (_log.Count > 0)
                            {
                                while (_log.Count > 0)
                                {
                                    await writer.WriteLineAsync(_log.Dequeue());
                                }

                                await writer.FlushAsync();
                            }
                        }
                    });

                    var watch = Stopwatch.StartNew();
                    _log.Enqueue(_path);
                    await ProcessDirAsync(new DirectoryInfo(_path));
                    watch.Stop();

                    string report = string.Join(Environment.NewLine, new string[]
                    {
                        "",
                        $"Execution Time: {watch.Elapsed}",
                        $"Levels: {_totalLevels}, Dirs: {_totalDirs}, Files: {_totalFiles}, Clean: {_totalClean}, Errors: {_totalErrors}"
                    });

                    _log.Enqueue(report);

                    Console.WriteLine(report);
                    Console.WriteLine($"Log: {_logFileName}");

                    run = false;

                    while (_log.Count > 0)
                    {
                        await writer.WriteLineAsync(_log.Dequeue());
                    }

                    await writer.FlushAsync();
                }

                //Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        #region AppSettings
        private static bool GetBoolean(string key)
        {
            string value = ConfigurationManager.AppSettings[key];
            return value.Equals("1");
        }

        private static string GetString(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        private static string[] GetMultiString(string key)
        {
            string value = ConfigurationManager.AppSettings[key];
            return value.Split(';').Select(n => n.Trim()).ToArray();
        }

        private static string[] GetStrings(string key)
        {
            string value = ConfigurationManager.AppSettings[key];
            return value.Split('\n').Select(n => n.Trim()).Where(n => !string.IsNullOrEmpty(n)).ToArray();
        }

        private static string GetLogFileName(string key)
        {
            string value = ConfigurationManager.AppSettings[key];
            var now = DateTime.Now;
            string path = Directory.CreateDirectory(Path.Combine(value, $"{now:yyyy}")).FullName;
            return Path.Combine(path, $"{now:yyyyMMdd-HHmm}.log");
        }
        #endregion AppSettings

        private static async Task ProcessDirAsync(DirectoryInfo di)
        {
            _totalDirs++;
            string path = di.FullName;

            //_log.Enqueue(path);

            foreach (var skip in _skips)
            {
                if (path.StartsWith(skip))
                {
                    Console.WriteLine($"{path} [skip]");
                    return;
                }
            }

            //if (_level < 3)
            //{
                Console.WriteLine(path);
            //}
            //else if (_level == 3)
            //{
            //    Console.WriteLine(path + "...");
            //}

            foreach (var mask in _masks)
            {
                foreach (var fi in di.EnumerateFiles(mask))
                {
                    ProcessFile(fi);
                }
            }

            while (_log.Count > 0)
            {
                await Task.Delay(100);
            }

            if (_subdirs)
            {
                _level++;
                _totalLevels = Math.Max(_totalLevels, _level);

                foreach (var sdi in di.EnumerateDirectories())
                {
                    try
                    {
                        await ProcessDirAsync(sdi);
                    }
                    catch (Exception e)
                    { 
                        Console.WriteLine(e.Message);
                    }
                }

                _level--;
            }
        }

        private static void ProcessFile(FileInfo fi)
        {
            _totalFiles++;

            var sb = new StringBuilder();

            bool modified = false;
            bool excepted = false;
            var lastWritten = fi.LastWriteTime;
            var readOnly = fi.IsReadOnly;

            if (readOnly)
            {
                if (_readonly)
                {
                    fi.IsReadOnly = false;
                }
                else
                {
                    sb.Append(" !ReadOnly.");
                }
            }

            var file = ShellFile.FromFilePath(fi.FullName);
            var prop = file.Properties.System;

            try
            {
                if (prop.Author.Value != null)
                {
                    sb.Append($" [Author: {string.Join("; ", prop.Author.Value)}]");

                    prop.Author.ClearValue();
                    modified = true;
                }

                if (prop.Category.Value != null)
                {
                    sb.Append($" [Category: {string.Join("; ", prop.Category.Value)}]");

                    prop.Category.ClearValue();
                    modified = true;
                }

                if (prop.Comment.Value != null)
                {
                    sb.Append($" [Comment: {prop.Comment.Value}]");

                    prop.Comment.ClearValue();
                    modified = true;
                }

                if (prop.Company.Value != null)
                {
                    sb.Append($" [Company: {prop.Company.Value}]");

                    prop.Company.ClearValue();
                    modified = true;
                }

                if (prop.ContentStatus.Value != null)
                {
                    sb.Append($" [ContentStatus: {prop.ContentStatus.Value}]");

                    prop.ContentStatus.ClearValue();
                    modified = true;
                }

                if (prop.Document.LastAuthor.Value != null)
                {
                    sb.Append($" [LastAuthor: {prop.Document.LastAuthor.Value}]");

                    prop.Document.LastAuthor.ClearValue();
                    modified = true;
                }

                if (prop.Document.Manager.Value != null)
                {
                    sb.Append($" [Manager: {prop.Document.Manager.Value}]");

                    prop.Document.Manager.ClearValue();
                    modified = true;
                }

                if (prop.Keywords.Value != null)
                {
                    sb.Append($" [Keywords: {string.Join("; ", prop.Keywords.Value)}]");

                    prop.Keywords.ClearValue();
                    modified = true;
                }

                if (prop.Subject.Value != null)
                {
                    sb.Append($" [Subject: {prop.Subject.Value}]");

                    prop.Subject.ClearValue();
                    modified = true;
                }

                if (prop.Title.Value != null)
                {
                    sb.Append($" [Title: {prop.Title.Value}]");

                    prop.Title.ClearValue();
                    modified = true;
                }
            }
            catch (Exception) //catch (Exception ex)
            {
                _totalErrors++;
                //sb.Append(" !Unwritable."); //.Append(ex.Message);
                modified = true;
                excepted = true;
            }

            if (modified)
            {
                try
                {
                    fi.LastWriteTime = lastWritten;
                    _totalClean++;
                }
                catch (Exception) //catch (Exception ex)
                {
                    if (excepted)
                    {
                        sb.Append(" !In use.");
                    }
                    else
                    {
                        _totalErrors++;
                        //sb.Append(" !").Append(ex.Message);
                        sb.Append(" !Unwritable.");
                    }
                }

                _log.Enqueue($"\"{fi.FullName}\"{sb}");
                Console.WriteLine($"  {fi.Name}{sb}");
            }

            if (readOnly && _readonly)
            {
                fi.IsReadOnly = true;
            }
        }
    }
}
