using System;
using System.Text;

namespace vILT.Domain
{
    public static class Base64StringExtensions
    {
        public static string ToDecodedBase64String(this string val)
        {
            var base64EncodedBytes = Convert.FromBase64String(val);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
