using System;
using System.Text;

namespace ConversionTools
{
    public static class Base64
    {
        /// <summary>
        /// Encodes a string to Base64
        /// </summary>
        /// <param name="input">UTF8 string</param>
        public static string Encode(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return "";
            }

            return Convert.ToBase64String(Encoding.UTF8.GetBytes(input));
        }

        public static string Decode(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return "";
            }

            return Encoding.UTF8.GetString(Convert.FromBase64String(input));
        }
    }
}
