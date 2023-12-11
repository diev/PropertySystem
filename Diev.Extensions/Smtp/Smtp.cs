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
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Text.Json;

using Diev.Extensions.Settings;

namespace Diev.Extensions.Smtp;

public class Smtp : IDisposable
{
    private readonly SmtpSettings _settings = new();
    private readonly SmtpClient? _client;
    //private ConcurrentQueue<MailMessage> _queue = new();

    public string DisplayName { get; set; } = Environment.MachineName;
    public List<string>? Subscribers { get; set; }

    public Smtp()
    {
        string path = Path.Combine(App.CompanyData, "SMTP.json");

        try
        {
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                _settings = JsonSerializer.Deserialize<SmtpSettings>(json)!;

                _client = new(_settings.Host, _settings.Port)
                {
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(
                        _settings.UserName,
                        SettingsManager.GetPassword(_settings.Password)),
                    EnableSsl = _settings.UseTls
                };
            }
            else
            {
                string json = JsonSerializer.Serialize(_settings);
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                File.WriteAllText(path, json);

                throw new Exception($"SMTP settings in '{path}' failed.");
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"SMTP settings in '{path}' failed.", ex);
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing && _client != null)
        {
            _client.Dispose();
        }
    }

    public async Task SendMessageAsync(string subj, string body, string[]? files)
    //public void SendMessage(string subj, string body, IEnumerable<string> files)
    {
        if (Subscribers is null || _client is null)
        {
            return;
        }

        try
        {
            using MailMessage mail = new()
            {
                From = new(_settings.UserName, DisplayName, Encoding.UTF8),
                Subject = subj,
                Body = body
            };

            foreach (var email in Subscribers)
            {
                mail.To.Add(email);
            }

            if (files != null)
            {
                foreach (var file in files)
                {
                    FileInfo fi = new(file);

                    if (fi.Exists)
                    {
                        Attachment attachment = new(fi.FullName);
                        ContentDisposition disposition = attachment.ContentDisposition!;

                        disposition.CreationDate = fi.CreationTime;
                        disposition.ModificationDate = fi.LastWriteTime;
                        disposition.ReadDate = fi.LastAccessTime;

                        mail.Attachments.Add(attachment);
                    }
                }
            }

            await _client.SendMailAsync(mail);
            //_queue.Enqueue(mail);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Sending mail '{subj}' failed.");
            Console.WriteLine(ex.ToString());
        }
    }
}
