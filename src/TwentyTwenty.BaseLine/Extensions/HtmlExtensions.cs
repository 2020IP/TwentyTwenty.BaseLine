using System.Text.RegularExpressions;

namespace TwentyTwenty.BaseLine
{
    public static class HtmlExtensions
    {
        public static string ConvertToPlainText(this string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            
            var htmlStrippedText = Regex.Replace(input, "<[^>]+>", " ")
                .Replace("&nbsp;", " ")
                .Replace("&amp;", "&")
                .Replace("&lt;", "<")
                .Replace("&gt;", ">")
                .Trim();
                
            // Replace multiple spaces created by regex replace with single space
            return htmlStrippedText = Regex.Replace(htmlStrippedText, "[ ]{2,}", " ");
        }
    }
}