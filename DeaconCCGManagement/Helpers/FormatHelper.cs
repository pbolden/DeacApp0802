using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DeaconCCGManagement.Services;

namespace DeaconCCGManagement.Helpers
{
    public static class FormatHelper
    {
        /// <summary>
        /// Formats phone number. 
        /// eg. 5551234567 --> (555) 123-4567
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string FormatPhoneNumber(string number)
        {
            if (number == null) { return null; }

            if (IsPhoneNumberFormatted(number))
            {
                return number;
            }
            var chars = number.Where(n => char.IsDigit(n)).ToList();
            if (chars.Count == 11)
            {
                // truncate the '1' at start if exists
                // eg. 15551239999 --> 5551239999
                chars.RemoveAt(0);
            }
            var sb = new StringBuilder();
            string numberStr = sb.Append(chars.ToArray()).ToString();
            sb.Clear();
            sb.Append("(");
            sb.Append(numberStr.Substring(0, 3));
            sb.Append(") ");
            sb.Append(numberStr.Substring(3, 3));
            sb.Append("-");
            sb.Append(numberStr.Substring(6, 4));

            return sb.ToString();
        }

        public static bool IsPhoneNumberFormatted(string number)
        {
            return string.IsNullOrEmpty(CcgMembersService.ValidatePhoneNumber(number));
        }
    }
}