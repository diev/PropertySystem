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
/// Класс работы с утилитой командной строки СКЗИ "КриптоПРО CSP".
/// </summary>
public static class CryptoPro
{
    /// <summary>
    /// Исполняемый файл командной строки.
    /// </summary>
    public static string Exe { get; set; } = @"C:\Program Files\Crypto Pro\CSP\csptest.exe";

    /// <summary>
    /// Отпечаток своего сертификата.
    /// </summary>
    public static string? My { get; set; }

    /// <summary>
    /// Пароль своего сертификата.
    /// </summary>
    public static string? PIN { get; set; }

    /// <summary>
    /// Команда подписи.
    /// {0} - исходный файл;
    /// {1} - подписанный файл;
    /// {2} - отпечаток своего сертификата.
    /// </summary>
    public static string SignCommand { get; set; } = "-sfsign -sign -silent -in {0} -out {1} -my {2} -add -addsigtime";

    /// <summary>
    /// Команда отсоединенной подписи.
    /// {0} - исходный файл;
    /// {1} - файл отсоединенной подписи;
    /// {2} - отпечаток своего сертификата.
    /// </summary>
    public static string SignDetachedCommand { get; set; } = "-sfsign -sign -silent -in {0} -out {1} -my {2} -add -addsigtime -detached";

    /// <summary>
    /// Команда проверки и снятия подписи.
    /// {0} - исходный файл;
    /// {1} - чистый файл;
    /// {2} - отпечаток своего сертификата.
    /// </summary>
    public static string VerifyCommand { get; set; } = "-sfsign -verify -silent -in {0} -out {1} -my {2}";

    /// <summary>
    /// Команда проверки отсоединенной подписи.
    /// {0} - исходный файл;
    /// {1} - файл отсоединенной подписи;
    /// {2} - отпечаток своего сертификата.
    /// </summary>
    public static string VerifyDetachedCommand { get; set; } = "-sfsign -verify -silent -in {0} -signature {1} -my {2} -detached";

    /// <summary>
    /// Команда шифрования.
    /// {0} - исходный файл;
    /// {1} - зашифрованный файл;
    /// {2} - отпечаток своего сертификата;
    /// {2} - отпечаток сертификата получателя.
    /// </summary>
    public static string EncryptCommand { get; set; } = "-sfenc -encrypt -silent -stream -1215gh -in {0} -out {1} -cert {2} -cert {3}";

    /// <summary>
    /// Команда расшифрования.
    /// {0} - исходный файл;
    /// {1} - расшифрованный файл;
    /// {2} - отпечаток своего сертификата.
    /// </summary>
    public static string DecryptCommand { get; set; } = "-sfenc -decrypt -silent -in {0} -out {1} -my {2}";

    /// <summary>
    /// Извлечь из PKCS#7 с ЭП чистый исходный текст.
    /// Криптопровайдер и проверка ЭП здесь не используются - только извлечение блока данных из формата ASN.1
    /// </summary>
    /// <param name="data">Массив байтов с сообщением в формате PKCS#7.</param>
    /// <returns>Массив байтов с исходным сообщением без ЭП.</returns>
    //public static byte[] CleanSign(byte[] data)
    //{
    //    var signedCms = new SignedCms();
    //    signedCms.Decode(data);
    //
    //    return signedCms.ContentInfo.Content;
    //}

    /// <summary>
    /// Подписать файл.
    /// </summary>
    /// <param name="file">Имя исходного файла.</param>
    /// <param name="resultFile">Имя подписанного файла.</param>
    /// <exception cref="ApplicationException"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    public static async Task SignFileAsync(string file, string resultFile)
    {
        if (My is null)
        {
            throw new ApplicationException("My cert thumbprint required.");
        }

        string cmdline = string.Format(SignCommand, file, resultFile, My);

        if (PIN != null)
        {
            cmdline += " -password " + PIN;
        }

        await StartAsync(Exe, cmdline, false);

        if (!File.Exists(resultFile))
        {
            throw new FileNotFoundException("Signed file not created.", resultFile);
        }
    }

    /// <summary>
    /// Подписать файл отсоединенной подписью.
    /// </summary>
    /// <param name="file">Имя исходного файла.</param>
    /// <param name="resultFile">Имя подписанного файла.</param>
    /// <exception cref="ApplicationException"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    public static async Task SignDetachedFileAsync(string file, string resultFile)
    {
        if (My is null)
        {
            throw new ApplicationException("My cert thumbprint required.");
        }

        string cmdline = string.Format(SignDetachedCommand, file, resultFile, My);

        if (PIN != null)
        {
            cmdline += " -password " + PIN;
        }

        await StartAsync(Exe, cmdline, false);

        if (!File.Exists(resultFile))
        {
            throw new FileNotFoundException("Detached sign file not created.", resultFile);
        }
    }

    /// <summary>
    /// Проверить и снять подпись с файла.
    /// </summary>
    /// <param name="file">Имя исходного файла.</param>
    /// <param name="resultFile">Имя файла без подписи.</param>
    /// <returns>Результат проверки подписи.</returns>
    /// <exception cref="ApplicationException"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    public static async Task<bool> VerifyFileAsync(string file, string resultFile)
    {
        if (My is null)
        {
            throw new ApplicationException("My cert thumbprint required.");
        }

        string cmdline = string.Format(VerifyCommand, file, resultFile, My);
        int exit = await StartAsync(Exe, cmdline, false);

        if (!File.Exists(resultFile))
        {
            throw new FileNotFoundException("Unsigned file not created.", resultFile);
        }

        return exit == 0;
    }

    /// <summary>
    /// Проверить отдельную подпись файла.
    /// </summary>
    /// <param name="file">Имя исходного файла.</param>
    /// <param name="signFile">Имя файла отдельной подписи.</param>
    /// <returns>Результат проверки подписи.</returns>
    /// <exception cref="ApplicationException"></exception>
    public static async Task<bool> VerifyDetachedFileAsync(string file, string signFile)
    {
        if (My is null)
        {
            throw new ApplicationException("My cert thumbprint required.");
        }

        string cmdline = string.Format(VerifyCommand, file, signFile, My);
        return await StartAsync(Exe, cmdline, false) == 0;
    }

    /// <summary>
    /// Зашифровать файл.
    /// </summary>
    /// <param name="file">Имя исходного файла.</param>
    /// <param name="resultFile">Имя зашифрованного файла.</param>
    /// <param name="to">Отпечаток сертификата получателя файла.</param>
    /// <exception cref="ApplicationException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    public static async Task EncryptFileAsync(string file, string resultFile, string? to)
    {
        if (My is null)
        {
            throw new ApplicationException("My cert thumbprint required.");
        }

        if (string.IsNullOrWhiteSpace(to))
        {
            throw new ArgumentNullException(nameof(to),
                "To cert thumbprint required.");
        }

        // Файлы более 256k (или все) должны шифроваться потоковым методом (-stream)
        // Файлы должны шифроваться по ГОСТ Р 34.12-2015 Кузнечик (-1215gh)
        string cmdline = string.Format(EncryptCommand, file, resultFile, My, to);
        await StartAsync(Exe, cmdline, false);

        if (!File.Exists(resultFile))
        {
            throw new FileNotFoundException("Encrypted file not created.", resultFile);
        }
    }

    /// <summary>
    /// Расшифровать файл.
    /// </summary>
    /// <param name="file">Имя исходного файла.</param>
    /// <param name="resultFile">Имя расшифрованного файла.</param>
    /// <exception cref="ApplicationException"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    public static async Task DecryptFileAsync(string file, string resultFile)
    {
        if (My is null)
        {
            throw new ApplicationException("My cert thumbprint required.");
        }

        string cmdline = string.Format(DecryptCommand, file, resultFile, My);

        if (PIN != null)
        {
            cmdline += " -password " + PIN;
        }

        await StartAsync(Exe, cmdline, false);

        if (!File.Exists(resultFile))
        {
            throw new FileNotFoundException("Decrypted file not created.", resultFile);
        }
    }
}
