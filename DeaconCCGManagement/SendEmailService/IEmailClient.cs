using DeaconCCGManagement.ViewModels;
using SendGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeaconCCGManagement.SendEmailService
{ 
    public interface IEmailClient
    {        
        Task<Response> SendSingleEmailAsync(EmailContact email);
        Task<Response> SendMultipleEmailsAsync(List<EmailContact> toAddressess, EmailContact email);
    }
}
