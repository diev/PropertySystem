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

using System.Diagnostics;

namespace Diev.Extensions.Exec;

public static class Exec
{
    /// <summary>
    /// Init required
    /// </summary>
    private static bool _init = true;

    /// <summary>
    /// Запустить программу с параметрами и дождаться ее завершения.
    /// </summary>
    /// <param name="exe">Запускаемая программа.</param>
    /// <param name="cmdline">Параметры для запускаемой программы.</param>
    /// <exception cref="FileNotFoundException"></exception>
    /// <exception cref="SystemException"></exception>
    public static void Start(string exe, string cmdline)
    {
        if (_init)
        {
            if (!File.Exists(exe))
            {
                throw new FileNotFoundException("File to exec not found.", exe);
            }

            _init = false;
        }

        ProcessStartInfo startInfo = new()
        {
            CreateNoWindow = false,
            WindowStyle = ProcessWindowStyle.Normal,
            UseShellExecute = true,
            FileName = exe,
            Arguments = cmdline
        };

        try
        {
            using Process? process = Process.Start(startInfo);

            if (process is null)
            {
                throw new Exception("Fail to get starting process.");
            }
            else
            {
                process.WaitForExit();
            }
        }
        catch (Exception ex)
        {
            throw new SystemException($"Fail to start [\"{exe}\" {cmdline}]", ex);
        }
    }

    /// <summary>
    /// Запустить программу с параметрами и дождаться ее завершения.
    /// </summary>
    /// <param name="exe">Файл запускаемой программы.</param>
    /// <param name="cmdline">Параметры для запускаемой программы.</param>
    /// <param name="visible">Показывать ли окно запускаемой программы.</param>
    /// <returns>Код возврата из программы.</returns>
    /// <exception cref="FileNotFoundException"></exception>
    /// <exception cref="SystemException"></exception>
    public static async Task<int> StartAsync(string exe, string cmdline, bool visible)
    {
        if (_init)
        {
            if (!File.Exists(exe))
            {
                throw new FileNotFoundException("File to exec not found.", exe);
            }

            _init = false;
        }

        ProcessStartInfo startInfo = new()
        {
            CreateNoWindow = false,
            WindowStyle = visible ? ProcessWindowStyle.Normal : ProcessWindowStyle.Hidden,
            UseShellExecute = false,
            FileName = exe,
            Arguments = cmdline
        };

        try
        {
            using Process? process = Process.Start(startInfo);

            if (process is null)
            {
                throw new Exception("Fail to get starting process.");
            }
            else
            {
                await process.WaitForExitAsync();
                return process.ExitCode;
            }
        }
        catch (Exception ex)
        {
            throw new SystemException($"Fail to start [\"{exe}\" {cmdline}]", ex);
        }
    }
}
