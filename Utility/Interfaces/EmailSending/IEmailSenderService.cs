using Data.Entities;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Utility.Interfaces.EmailSending
{
    public interface IEmailSenderService : IEmailSender
    {
        public Task SendVacancieRespondChangedStatus(VacancieRespond respond);
    }
}
