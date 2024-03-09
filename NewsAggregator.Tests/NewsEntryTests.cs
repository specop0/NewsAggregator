using System;
using System.Linq;
using NewsAggregator.Database;
using Newtonsoft.Json;
using NUnit.Framework;

namespace NewsAggregator.Tests;
public class NewsEntryTests : TestsBase
{
    [Test]
    public void TestSerialization()
    {
        var url = this.GetUniqueName("https://url.");
        var title = this.GetUniqueName("Title");
        var summary = this.GetUniqueName("Summary");
        var imageUrl = this.GetUniqueName("https://img.");

        var original = new NewsEntry(url, title, summary, imageUrl);
        Assert.That(original.Url, Is.EqualTo(url));
        Assert.That(original.Title, Is.EqualTo(title));
        Assert.That(original.Summary, Is.EqualTo(summary));
        Assert.That(original.ImageUrl, Is.EqualTo(imageUrl));

        var json = JsonConvert.SerializeObject(original);
        var copy = JsonConvert.DeserializeObject<NewsEntry>(json) ?? throw new NullReferenceException();
        Assert.That(copy.Url, Is.EqualTo(url));
        Assert.That(copy.Title, Is.EqualTo(title));
        Assert.That(copy.Summary, Is.EqualTo(summary));
        Assert.That(copy.ImageUrl, Is.EqualTo(imageUrl));

        Assert.That(copy, Is.EqualTo(original));
    }

    [Test]
    public void TestEquality()
    {
        var url = this.GetUniqueName("https://url.");
        var title = this.GetUniqueName("Title");
        var summary = this.GetUniqueName("Summary");
        var imageUrl = this.GetUniqueName("https://img.");

        var left = new NewsEntry(url, title, summary, imageUrl);
        var right = new NewsEntry(url, title, summary, imageUrl);

        Assert.That(left, Is.EqualTo(right));
        Assert.That(left.Equals(right), Is.True);
        Assert.That(left == right, Is.True);
        Assert.That(left != right, Is.False);
        Assert.That(left.ToString(), Is.EqualTo(right.ToString()));
        Assert.That(left.GetHashCode(), Is.EqualTo(right.GetHashCode()));

        var propertyNames = new[] {
                nameof(NewsEntry.Url),
                nameof(NewsEntry.Title),
                nameof(NewsEntry.Summary),
                nameof(NewsEntry.ImageUrl),
            };
        var ignoredPropertyNames = new[] {
                nameof(NewsEntry.ImageUrl),
            };
        foreach (var propertyName in propertyNames)
        {
            var other = new NewsEntry(
                propertyName == nameof(NewsEntry.Url) ? url + "x" : url,
                propertyName == nameof(NewsEntry.Title) ? title + "x" : title,
                propertyName == nameof(NewsEntry.Summary) ? summary + "x" : summary,
                propertyName == nameof(NewsEntry.ImageUrl) ? imageUrl + "x" : imageUrl
            );

            var isEqual = ignoredPropertyNames.Contains(propertyName);
            var assertEqual = isEqual
                ? new Action<object, object>((a, b) => Assert.That(a, Is.EqualTo(b)))
                : (a, b) => Assert.That(a, Is.Not.EqualTo(b));
            assertEqual(left, other);
            Assert.That(left.Equals(other), Is.EqualTo(isEqual));
            Assert.That(left == other, Is.EqualTo(isEqual));
            Assert.That(left != other, Is.EqualTo(!isEqual));
            assertEqual(left.GetHashCode(), other.GetHashCode());
            Assert.That(left.ToString(), Is.Not.EqualTo(other.ToString()));
        }
    }

    [Test]
    public void TestEuqalityNullSafe()
    {
        var entry = new NewsEntry(null, null, null, null);
        NewsEntry? nullEntry = null;

        Assert.That(entry, Is.Not.EqualTo(nullEntry));
        Assert.That(entry, Is.EqualTo(entry));
        Assert.That(nullEntry, Is.EqualTo(nullEntry));

#pragma warning disable CS1718
        Assert.That(entry == nullEntry, Is.False);
        Assert.That(nullEntry == entry, Is.False);
        Assert.That(entry == entry, Is.True);
        Assert.That(nullEntry == nullEntry, Is.True);

        Assert.That(entry != nullEntry, Is.True);
        Assert.That(nullEntry != entry, Is.True);
        Assert.That(entry != entry, Is.False);
        Assert.That(nullEntry != nullEntry, Is.False);
#pragma warning restore CS1718
    }

    [Test]
    public void TestTrimWhitespaces()
    {
        var first = this.GetUniqueName();
        var middle = this.GetUniqueName();
        var end = this.GetUniqueName();
        var title = $"  {first}   {middle} {end}     ";
        var expectedTitle = $"{first}   {middle} {end}";
        var summary = $" {end} {middle}  {first}    ";
        var expectedSummary = $"{end} {middle}  {first}";
        var newsEntry = new NewsEntry(null, title, summary, null);
        Assert.That(newsEntry.Title, Is.EqualTo(expectedTitle));
        Assert.That(newsEntry.Summary, Is.EqualTo(expectedSummary));
    }
}
