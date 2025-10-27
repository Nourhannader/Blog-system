namespace Blog.Helpers
{
    public class HtmlTagHelper
    {
        public static string RemoveHtmlTags(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }
            // Use a regular expression to remove HTML tags
            return System.Text.RegularExpressions.Regex.Replace(input, "<.*?>|&.*?;", string.Empty);
        }
    }
}
