using System.Text.RegularExpressions;

namespace TwentyTwenty.BaseLine
{
    public static class StringExtensions
    {
        public static string RemoveControlCharacters(this string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            
            return Regex.Replace(input, @"\p{Cc}+", string.Empty);
        }
    }
}