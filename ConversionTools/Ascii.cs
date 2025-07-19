using System.Text;

namespace ConversionTools
{
    public static class Ascii
    {
        /// <summary>
        /// Encodes a string to Ascii
        /// </summary>
        /// <param name="input"></param>
        /// <param name="useUrlEcoding">URL ready format<br/>e.g. 32 becomes &#32;</param>
        /// <returns></returns>
        public static string Encode(string input, bool useUrlEcoding = true)
        {
            if (string.IsNullOrEmpty(input))
            {
                return "";
            }

            StringBuilder sb = new();
            foreach (char c in input)
            {
                if (useUrlEcoding)
                {
                    sb.Append($"&#{((int)c)};");
                    continue;
                }
                sb.Append($"{((int)c)} ");
            }

            return sb.ToString().Trim();
        }

        public static string Decode(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return "";
            }

            StringBuilder sb = new();

            if (input.Contains("&#"))
            {
                input = input.Replace("&#", "").Replace(";", " ");
            }

            string[] parts = input.Split(' ');
            foreach (string part in parts)
            {
                if (!int.TryParse(part, out int di))
                {
                    continue;
                }

                if (part.Length > 0)
                {
                    sb.Append((char)di);
                }
            }
            return sb.ToString();
        }
    }
}
