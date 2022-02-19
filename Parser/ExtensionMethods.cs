namespace Parser
{
    using System.Collections.Generic;
    using HtmlAgilityPack;
    public static class ExtensionMethods
    {
        public static bool TryGetValue(this HtmlAttributeCollection attributes, string key, out string value)
        {
            value = string.Empty;

            if (attributes.Contains(key))
            {
                value = attributes[key].Value;
                return true;
            }

            return false;
        }

        public static void AddRange<T>(this HashSet<T> target, IEnumerable<T> source)
        {
            foreach (var item in source)
            {
                target.Add(item);
            }
        }

        public static string ToHtml(this System.Collections.Generic.IEnumerable<NewsEntry> newsEntries)
        {
            var builder = new System.Text.StringBuilder();
            var htmlDocument = new HtmlDocument();
            builder.AppendLine("<html>");
            builder.AppendLine("<head>");
            builder.AppendLine("<title>Nachrichten</title>");
            builder.AppendLine("<style>");
            builder.AppendLine("body { background: lightgray; }");
            builder.AppendLine("div { margin: 50px; }");
            builder.AppendLine("a { text-decoration: none; }");
            builder.AppendLine("img { max-width: 60%; min-width: 600px; }");
            builder.AppendLine(".title { font-weight: bold; }");
            builder.AppendLine("@media only screen and (min-width: 1000px) { p { width: 50%; } }");
            builder.AppendLine("</style>");
            builder.AppendLine("</head>");

            builder.AppendLine("<body>");
            foreach (var news in newsEntries)
            {
                if (news == null)
                {
                    builder.AppendLine("<hr>");
                    continue;
                }
                builder.AppendLine("<div>");
                builder.AppendLine($"<a href='{news.Url}'>");
                builder.AppendLine($"<img src='{news.ImageData ?? news.ImageUrl}'>");
                builder.AppendLine($"<p class='title'>{news.Title}</p>");
                builder.AppendLine($"<p>{news.Summary}</p>");
                builder.AppendLine($"</a>");
                builder.AppendLine("</div>");
            }
            builder.AppendLine("</body>");
            builder.AppendLine("</html>");
            return builder.ToString();
        }
    }
}