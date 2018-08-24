using System.Threading.Tasks;

namespace DeaconCCGManagement.SmsService
{
    public interface IResponse
    {
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        string Status { get; set; }
        /// <summary>
        /// Gets a value indicating whether this instance can update.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance can update; otherwise, <c>false</c>.
        /// </value>
        bool CanUpdate { get; }
        /// <summary>
        /// Updates the response asynchronously.
        /// </summary>
        /// <returns></returns>
        Task UpdateAsync();
    }
}
