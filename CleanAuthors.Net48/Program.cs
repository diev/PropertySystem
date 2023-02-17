#region License
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

using Microsoft.WindowsAPICodePack.Shell;

using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace CleanAuthors.Net48
{
    internal class Program
    {
        //app.config
        private static readonly string _path = ConfigurationManager.AppSettings["Path"];
        private static readonly string[] _masks = ConfigurationManager.AppSettings["Masks"].Split(new char[] { ';' });
        private static readonly bool _subdirs = ConfigurationManager.AppSettings["Subdirs"].Equals("1");
        private static readonly bool _readonly = ConfigurationManager.AppSettings["ReadOnly"].Equals("1");
        private static string _log = ConfigurationManager.AppSettings["Log"];
        private static readonly Encoding _enc = Encoding.GetEncoding(1251);

        static void Main(string[] args)
        {
            try
            {
                var logs = Directory.CreateDirectory(Path.Combine(_log, $"{DateTime.Now:yyyy}"));
                _log = Path.Combine(logs.FullName, $"{DateTime.Now:yyyyMMdd}.log");
                File.AppendAllText(_log, $"[{DateTime.Now:yyyy-MM-dd HH:mm}]\n", _enc);
                Console.WriteLine("Wait...");

                var watch = Stopwatch.StartNew();
                ProcessDir(new DirectoryInfo(_path));
                watch.Stop();

                Console.WriteLine($"Execution Time: {TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds)}");
                File.AppendAllText(_log, $"\n\n", _enc);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void ProcessDir(DirectoryInfo di)
        {
            Console.WriteLine(di.FullName);

            foreach (var mask in _masks)
            {
                foreach (var fi in di.EnumerateFiles(mask))
                {
                    ProcessFile(fi);
                }
            }

            if (_subdirs)
            {
                foreach (var sdi in di.EnumerateDirectories())
                {
                    try
                    {
                        ProcessDir(sdi);
                    }
                    catch (Exception e)
                    { 
                        Console.WriteLine(e.Message);
                    }
                }
            }
        }

        private static void ProcessFile(FileInfo fi)
        {
            var sb = new StringBuilder();

            bool modified = false;
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
                    sb.Append(" !ReadOnly");
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
            catch (Exception ex)
            {
                sb.Append(" !").Append(ex.Message);
                modified = true;
            }

            if (modified)
            {
                try
                {
                    fi.LastWriteTime = lastWritten;
                }
                catch (Exception ex)
                {
                    sb.Append(" !").Append(ex.Message);
                }

                Console.WriteLine($"  {fi.Name}{sb}");
                File.AppendAllText(_log, $"\"{fi.FullName}\"{sb}\n", _enc);
            }

            if (readOnly && _readonly)
            {
                fi.IsReadOnly = true;
            }
        }
    }
}
