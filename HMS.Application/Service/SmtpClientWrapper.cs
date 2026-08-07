using HMS.Application.Contracts.Service;
using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;

using System.Text;

namespace HMS.Application.Service
{
    public class SmtpClientWrapper : ISmtpClientWrapper
    {
        private readonly SmtpClient _client = new();

        public async Task AuthenticateAsync(string userName, string passWord) => await _client.AuthenticateAsync(userName, passWord);


        public async Task ConnectAsync(string host, int port, bool useSsl) => await _client.ConnectAsync(host, port, useSsl);

        public async Task SendAsync(MimeMessage message) => await _client.SendAsync(message);

        public async Task DisconnectAsync(bool quit) => await _client.DisconnectAsync(quit);

        public void Dispose() => _client.Dispose();
    }
}
