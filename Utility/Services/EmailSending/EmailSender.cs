﻿using Mailjet.Client;
using Mailjet.Client.Resources;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity.UI.Services;
using Utility.Settings;
using Data.Entities;
using Utility.Interfaces.EmailSending;

namespace Utility.Services.EmailSending
{
    public class EmailSenderService : IEmailSenderService
    {
        private readonly MailjetSettings _settings;
        private readonly MailjetClient _mailjetClient;

        public EmailSenderService(IOptions<MailjetSettings> options)
        {
            _settings = options.Value;
            _mailjetClient = new MailjetClient(_settings.ApiKeyPublic, _settings.ApiKeyPrivate);
        }

        /// <summary>
        /// Sends email via Mailjet API
        /// </summary>
        /// <param name="email">Recipient email</param>
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            MailjetRequest request = new MailjetRequest
            {
                Resource = Send.Resource,
            }
                .Property(Send.FromEmail, _settings.SenderEmail)
                .Property(Send.FromName, "JobHunt")
                .Property(Send.Subject, subject)
                .Property(Send.HtmlPart, htmlMessage)
                .Property(Send.Recipients, new JArray {
                    new JObject {
                        {"Email", email}
                    }});

            MailjetResponse response = await _mailjetClient.PostAsync(request);
        }

        public async Task SendVacancieRespondChangedStatus(VacancieRespond respond)
        {
            await SendEmailAsync(respond.Resume.Jobseeker.AppUser.Email, "Your respond was answered",
                $"You've got an answer on your respond with resume {respond.Resume}. Check it out!");
        }
    }
}
