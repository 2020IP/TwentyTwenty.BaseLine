using System.Linq;
using System.Text.RegularExpressions;

namespace TwentyTwenty.BaseLine
{
    public static class StringExtensions
    {
        private static char[] removableChars = @"\{^}%`]""'>[~<#|*&$@=;/:+,?".ToCharArray();

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

        public static string Sanitize(this string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            // TODO: Make removable chars a static char array that exists outside this method
            //       var removableChars = @"\{^}%`]""'>[~<#|*&$@=;/:+,?";
            return string.Concat(input.Where(c => !removableChars.Contains(c)))
                .RemoveAllControlCharacters()
                .RemoveNonAsciiCharacters();
        }
    }
}