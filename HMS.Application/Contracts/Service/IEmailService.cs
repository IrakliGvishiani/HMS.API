using HMS.Application.Models.Notification;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Application.Contracts.Service
{
    public interface IEmailService
    {
        Task<SendEmailResponse> Send(string to, string subject, string body);

    }
}
