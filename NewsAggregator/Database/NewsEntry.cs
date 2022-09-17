using System;

namespace NewsAggregator.Database;

public class NewsEntry : IEquatable<NewsEntry>
{
    public NewsEntry(string? url, string? title, string? summary, string? imageUrl)
    {
        this.Url = url ?? string.Empty;
        this.Title = (title ?? string.Empty).Trim();
        this.Summary = (summary ?? string.Empty).Trim();
        this.ImageUrl = imageUrl ?? string.Empty;
        this.ImageData = string.Empty;
    }

    public string Url { get; }
    public string Title { get; }
    public string Summary { get; }
    public string ImageUrl { get; }
    public string ImageData { get; set; }

    // VERTICAL LINE EXTENSION (U+23D0)
    private const string Separator = "⏐";

    public override string ToString()
    {
        return string.Join(
            Separator,
            this.Url,
            this.Title,
            this.Summary,
            this.ImageUrl);
    }

    public NewsEntry? FromString(string value)
    {
        var values = value.Split(new[] { Separator }, StringSplitOptions.None);
        if (values.Length == 4)
        {
            var url = values[0];
            var title = values[1];
            var summary = values[2];
            var imageUrl = values[3];

            return new NewsEntry(
                url,
                title,
                summary,
                imageUrl);
        }

        return null;
    }

    public override bool Equals(object? obj)
    {
        if (obj is NewsEntry other)
        {
            return this.Equals(other);
        }

        return false;
    }

    public bool Equals(NewsEntry? other)
    {
        if (object.ReferenceEquals(other, null))
        {
            return false;
        }

        return this.Url == other.Url
            && this.Title == other.Title
            && this.Summary == other.Summary;
    }

    public static bool operator ==(NewsEntry? left, NewsEntry? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(NewsEntry? left, NewsEntry? right)
    {
        return !Equals(left, right);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var result = 0;
            result = (result * 397) ^ this.Url.GetHashCode();
            result = (result * 397) ^ this.Title.GetHashCode();
            result = (result * 397) ^ this.Summary.GetHashCode();
            return result;
        }
    }
}
