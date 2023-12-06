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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;

using Task = System.Threading.Tasks.Task;

namespace CleanAuthors.Net48
{
    internal class Program
    {
        //app.config
        private static readonly string _path = AppSettings.GetString("Path");
        private static readonly string[] _skips = AppSettings.GetStrings("Skip");
        private static readonly string[] _masks = AppSettings.GetMultiString("Masks");
        private static readonly bool _subdirs = AppSettings.GetBoolean("Subdirs");
        private static readonly bool _readonly = AppSettings.GetBoolean("ReadOnly");
        private static readonly bool _logToConsole = AppSettings.GetBoolean("LogToConsole");
        private static readonly string _logFileName = AppSettings.GetLogFileName("Logs");

        //counters
        private static int _level = 0;
        private static int _totalLevels = 0;
        private static int _totalDirs = 0;
        private static int _totalFiles = 0;
        private static int _totalClean = 0;
        private static int _totalErrors = 0;

        private static async Task Main(string[] args)
        {
            try
            {
                WriteRunStatus("Start clean");
                Logger.Log = _logFileName;
                Logger.WriteLine(_path);

                if (!_logToConsole)
                {
                    Console.WriteLine(_path);
                    Console.WriteLine("LogToConsole: Off");
                }

                var watch = Stopwatch.StartNew();

                await ProcessDirAsync(new DirectoryInfo(_path));

                watch.Stop();

                string report = string.Join(Environment.NewLine, new string[]
                {
                    "",
                    $"Execution Time: {watch.Elapsed}",
                    $"Levels: {_totalLevels}, Dirs: {_totalDirs}, Files: {_totalFiles}, Clean: {_totalClean}, Errors: {_totalErrors}"
                });

                Logger.Timed = false;
                Logger.WriteLine(report);

                Console.WriteLine(report);
                Console.WriteLine($"Log: {Logger.Log}");

                //Console.ReadLine();
                WriteRunStatus("Stop clean 0");
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                WriteRunStatus("Stop clean 1");
                Environment.Exit(1);
            }
            finally
            {
                Logger.Close();
            }
        }

        private static void WriteRunStatus(string status = "Run")
        {
            string message = $"{DateTime.Now:G} {status}{Environment.NewLine}";

            try
            {
                string path = AppSettings.GetString("Logs");
                string log = Path.Combine(path, "startstop.log");

                File.AppendAllText(log, message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(message);
                Console.WriteLine(ex.Message);
            }
        }

        private static async Task ProcessDirAsync(DirectoryInfo di)
        {
            _totalDirs++;
            try
            {
                string path = di.FullName;

                foreach (var skip in _skips)
                {
                    if (path.StartsWith(skip))
                    {
                        Console.WriteLine($"{path} [skip]");
                        return;
                    }
                }

                if (_logToConsole)
                {
                    //if (_level < 3)
                    //{
                    Console.WriteLine(path);
                    //}
                    //else if (_level == 3)
                    //{
                    //    Console.WriteLine(path + "...");
                    //}
                }

                try
                {
                    foreach (var fi in _masks.SelectMany(mask => di.EnumerateFiles(mask)))
                    {
                        await ProcessFile(fi);
                    }
                }
                catch { }

                while (!Logger.IsEmpty)
                {
                    await Task.Delay(100);
                }

                if (_subdirs)
                {
                    _level++;
                    _totalLevels = Math.Max(_totalLevels, _level);

                    try
                    {
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
                    }
                    catch { }

                    _level--;
                }
            }
            catch { }
        }

        private static Task ProcessFile(FileInfo fi)
        {
            _totalFiles++;
            try
            {
                using (var file = ShellFile.FromFilePath(fi.FullName))
                {
                    var prop = file.Properties.System;
                    try
                    {
                        if (prop.Author.Value != null ||
                            prop.Category.Value != null ||
                            prop.Comment.Value != null ||
                            prop.Company.Value != null ||
                            prop.ContentStatus.Value != null ||
                            prop.Document.LastAuthor.Value != null ||
                            prop.Document.Manager.Value != null ||
                            prop.Keywords.Value != null ||
                            prop.Subject.Value != null ||
                            prop.Title.Value != null)
                        {
                            CleanProcessFile(fi, prop);
                        }
                    }
                    catch
                    {
                        CleanProcessFile(fi, prop);
                    }
                }
            }
            catch { }

            return Task.CompletedTask;
        }

        private static Task CleanProcessFile(FileInfo fi, ShellProperties.PropertySystem prop)
        {
            var sb = new StringBuilder();

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

            try
            {
                if (prop.Author.Value != null)
                {
                    sb.Append($" [Author: {string.Join("; ", prop.Author.Value)}]");
                    prop.Author.ClearValue();
                }

                if (prop.Category.Value != null)
                {
                    sb.Append($" [Category: {string.Join("; ", prop.Category.Value)}]");
                    prop.Category.ClearValue();
                }

                if (prop.Comment.Value != null)
                {
                    sb.Append($" [Comment: {prop.Comment.Value}]");
                    prop.Comment.ClearValue();
                }

                if (prop.Company.Value != null)
                {
                    sb.Append($" [Company: {prop.Company.Value}]");
                    prop.Company.ClearValue();
                }

                if (prop.ContentStatus.Value != null)
                {
                    sb.Append($" [ContentStatus: {prop.ContentStatus.Value}]");
                    prop.ContentStatus.ClearValue();
                }

                if (prop.Document.LastAuthor.Value != null)
                {
                    sb.Append($" [LastAuthor: {prop.Document.LastAuthor.Value}]");
                    prop.Document.LastAuthor.ClearValue();
                }

                if (prop.Document.Manager.Value != null)
                {
                    sb.Append($" [Manager: {prop.Document.Manager.Value}]");
                    prop.Document.Manager.ClearValue();
                }

                if (prop.Keywords.Value != null)
                {
                    sb.Append($" [Keywords: {string.Join("; ", prop.Keywords.Value)}]");
                    prop.Keywords.ClearValue();
                }

                if (prop.Subject.Value != null)
                {
                    sb.Append($" [Subject: {prop.Subject.Value}]");
                    prop.Subject.ClearValue();
                }

                if (prop.Title.Value != null)
                {
                    sb.Append($" [Title: {prop.Title.Value}]");
                    prop.Title.ClearValue();
                }

            }
            catch (Exception) //catch (Exception ex)
            {
                _totalErrors++;
                //sb.Append(" !Unwritable."); //.Append(ex.Message);
                excepted = true;
            }

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

            Logger.WriteLine($"\"{fi.FullName}\"{sb}");
            Console.WriteLine($"  {fi.Name}{sb}");

            if (readOnly && _readonly)
            {
                fi.IsReadOnly = true;
            }

            return Task.CompletedTask;
        }
    }
}
