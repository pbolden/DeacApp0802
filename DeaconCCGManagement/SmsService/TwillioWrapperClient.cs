using DeaconCCGManagement.Models;
using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace DeaconCCGManagement.SmsService

{
    public class TwillioWrapperClient : ISmsClient
    {
        public bool IsInitialized { get; set; }

        public bool CanSendSms { get { return true; } }

        public bool FromNumberRequired { get { return true; } }     

        public void Init()
        {
            var accountSid = ConfigurationManager.AppSettings["AccountSid"];
            var authToken = ConfigurationManager.AppSettings["AuthToken"];           
            TwilioClient.Init(accountSid, authToken);
            IsInitialized = true;
        }           

        public async Task<IResponse> SendSmsAsync(SmsMessage sms)
        {
            if (!IsInitialized) Init();

            if (string.IsNullOrEmpty(sms.FromNumber))
                sms.FromNumber = ConfigurationManager.AppSettings["TwillioFromNumber"];

            var fromNumber = new PhoneNumber(GetTextNumber(sms.FromNumber));
            var toNumber = new PhoneNumber(GetTextNumber(sms.ToNumber));

            var body = WebUtility.UrlEncode(sms.Message);         

            var text = await MessageResource.CreateAsync(
                from: fromNumber,
                to: toNumber,
                body: body);

            return new TextResponse(text);
        }

        public override string ToString()
        {
            return "Twillio API";
        }

        private string GetTextNumber(string number)
        {
            if (number == null) return null;

            var sb = new StringBuilder();
            if (number.Substring(0) != "+")
                sb.Append("+");
            if (number.Substring(1) != "1")
                sb.Append("1");            
            
            foreach (var n in number.Where(char.IsDigit))
                sb.Append(n);

            return sb.ToString();
        }

        public class TextResponse : IResponse
        {
            private string _sid;

            public string Status { get; set; }

            public bool CanUpdate { get { return true; } }

            public TextResponse(MessageResource text)
            {
                SetMessage(text);
            }

           private void SetMessage(MessageResource text)
            {
                _sid = text.Sid;
                Status = text.Status.ToString();
            }

            public async Task UpdateAsync()
            {
                var text = await MessageResource.FetchAsync(_sid);
            }
        }
    }
   
}