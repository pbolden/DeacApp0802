using DeaconCCGManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeaconCCGManagement.SmsService

{
    public interface ISmsClient
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance is initialized.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is initialized; otherwise, <c>false</c>.
        /// </value>
        bool IsInitialized { get; }
        /// <summary>
        /// Gets a value indicating whether this instance can send SMS.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance can send SMS; otherwise, <c>false</c>.
        /// </value>
        bool CanSendSms { get; }     
        /// <summary>
        /// Gets a value indicating whether [from number required].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [from number required]; otherwise, <c>false</c>.
        /// </value>
        bool FromNumberRequired { get; }
        /// <summary>
        /// Initializes this instance.
        /// </summary>
        void Init();      
        /// <summary>
        /// Sends the SMS asynchronous.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="msg">The MSG.</param>
        /// <returns></returns>
        Task<IResponse> SendSmsAsync(SmsMessage sms);
        /// <summary>
        /// The client name.
        /// </summary>
        string ToString();
    }
}
