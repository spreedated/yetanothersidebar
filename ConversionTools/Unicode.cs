using System.Linq;
using System.Text;
using System.Globalization;

namespace ConversionTools
{
    public static class Unicode
    {
        public static string Encode(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return "";
            }

            StringBuilder sb = new();

            foreach (char c in input)
            {
                sb.Append($"\\u{(int)c:x4}");
            }

            return sb.ToString();
        }

        public static string Decode(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return "";
            }

            string[] parts = [.. input.ToLower().Split(@"\u").Where(x => x.Length >= 4)];

            StringBuilder sb = new();

            foreach (string p in parts)
            {
                if (!int.TryParse(p[2..], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int res))
                {
                    continue;
                }

                sb.Append((char)res);
            }

            return sb.ToString();
        }
    }
}
