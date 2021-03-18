using System.Text.RegularExpressions;

namespace TwentyTwenty.BaseLine
{
    public static class StringExtensions
    {
        public static string RemoveBaseControlCharacters(this string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            
            return Regex.Replace(input, @"\p{Cc}+", string.Empty).Trim();
        }
        public static string RemoveAllControlCharacters(this string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            
            return Regex.Replace(input, @"\p{C}+", string.Empty).Trim();
        }

        public static string RemoveNonAsciiCharacters(this string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            return Regex.Replace(input, @"[^\u0000-\u007F]+", string.Empty).Trim();
        }
    }
}