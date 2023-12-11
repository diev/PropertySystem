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

using static Diev.Extensions.Exec.Exec;

namespace Diev.Extensions.Crypto;

/// <summary>
/// Класс работы с утилитой командной строки СКАД "Сигнатура".
/// </summary>
public static class SpkiUtl
{
    /// <summary>
    /// Исполняемый файл командной строки.
    /// </summary>
    public static string Exe { get; set; } = @"C:\Program Files\MDPREI\spki\spki1utl.exe";

    /// <summary>
    /// Пароль своего сертификата.
    /// </summary>
    public static string? PIN { get; set; }

    /// <summary>
    /// Команда подписи.
    /// {0} - исходный файл;
    /// {1} - подписанный файл.
    /// </summary>
    public static string SignCommand { get; set; } = "-sign -data {0} -out {1}";

    /// <summary>
    /// Команда отсоединенной подписи.
    /// {0} - исходный файл;
    /// {1} - файл отсоединенной подписи.
    /// </summary>
    public static string SignDetachedCommand { get; set; } = "-sign -data {0} -out {1} -detached";

    /// <summary>
    /// Команда проверки и снятия подписи.
    /// {0} - исходный файл;
    /// {1} - чистый файл.
    /// </summary>
    public static string VerifyCommand { get; set; } = "-verify -in {0} -out {1} -delete 1";

    /// <summary>
    /// Команда проверки отсоединенной подписи.
    /// {0} - исходный файл;
    /// {1} - файл отсоединенной подписи.
    /// </summary>
    public static string VerifyDetachedCommand { get; set; } = "-verify -in {0} -signature {1} -detached"; //TODO

    /// <summary>
    /// Команда шифрования.
    /// {0} - исходный файл;
    /// {1} - зашифрованный файл;
    /// {2} - номер сертификата получателя.
    /// </summary>
    public static string EncryptCommand { get; set; } = "-encrypt -stream -1215gh -1215mac -in {0} -out {1} -reckeyid {2}";

    /// <summary>
    /// Команда расшифрования.
    /// {0} - исходный файл;
    /// {1} - расшифрованный файл.
    /// </summary>
    public static string DecryptCommand { get; set; } = "-decrypt -in {0} -out {1}";

    /// <summary>
    /// Создание отсоединенной электронной подписи с помощью утилиты командной строки.
    /// </summary>
    /// <param name="sourceFileName">Исходный файл.</param>
    /// <param name="p7d">Двоичный файл создаваемой электронной подписи.</param>
    /// <param name="overwrite">Переписывать ли файл, если он уже есть.</param>
    /// <returns>Создан ли файл электронной подписи.</returns>
    public static bool CreateSignDetached(string sourceFileName, string p7d, bool overwrite = false)
    {
        if (File.Exists(p7d))
        {
            if (overwrite)
            {
                File.Delete(p7d);
            }
            else
            {
                return true;
            }
        }

        string cmdline = string.Format(SignDetachedCommand, sourceFileName, p7d);
        //Task.Run(async () =>
        //{
        //    await StartAsync(Exe, cmdline); 
        //});
        Start(Exe, cmdline);

        return File.Exists(p7d);
    }
}
