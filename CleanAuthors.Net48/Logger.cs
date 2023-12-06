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
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CleanAuthors.Net48
{
    public static class Logger
    {
        private static string _file;
        private static FileStream _stream = null;
        private static StreamWriter _writer;

        private static readonly ConcurrentQueue<string> _strings = new ConcurrentQueue<string>();
        private static readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();
        private static CancellationToken _token;
        private static Task _task;

        public static int Count => _strings.Count;
        public static bool IsEmpty => _strings.IsEmpty;
        public static bool Timed { get; set; } = true;

        public static string Log
        {
            get => _file;
            set
            {
                _file = value;
                Directory.CreateDirectory(Path.GetDirectoryName(_file));

                _stream = new FileStream(_file, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read, 4096, true);
                _writer = new StreamWriter(_stream, Encoding.GetEncoding(1251));

                _token = _tokenSource.Token;
                _task = Task.Run(WriteAsync, _token);
            }
        }

        public static void Close()
        {
            if (_stream != null)
            {
                Task.Run(CloseAsync).Wait();
                _writer.Close();
            }
        }

        public static void WriteLine(string message)
        {
            if (Timed)
            {
                message = $"{DateTime.Now:u} {message}";
            }

            _strings.Enqueue(message);
        }

        private static async Task WriteAsync()
        {
            while (!_token.IsCancellationRequested)
            {
                if (!_strings.IsEmpty)
                {
                    while (_strings.TryDequeue(out string message))
                    {
                        await _writer.WriteLineAsync(message);
                    }

                    await _writer.FlushAsync();
                }

                Thread.Sleep(100);
            }
        }

        private static async Task CloseAsync()
        {
            _tokenSource.Cancel();
            _task.Wait();

            while (_strings.TryDequeue(out string message))
            {
                await _writer.WriteLineAsync(message);
            }

            await _writer.FlushAsync();
        }
    }
}
