using System;
using System.Text.Encodings.Web;

namespace ConversionTools
{
    public static class UrlEncode
    {
        public static string Encode(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return "";
            }

            return UrlEncoder.Default.Encode(input).ToLower();
        }

        public static string Decode(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return "";
            }

            return Uri.UnescapeDataString(input);
        }
    }
}
