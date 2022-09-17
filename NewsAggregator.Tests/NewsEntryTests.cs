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

        var expected = new NewsEntry(url, title, summary, imageUrl);
        Assert.AreEqual(url, expected.Url);
        Assert.AreEqual(title, expected.Title);
        Assert.AreEqual(summary, expected.Summary);
        Assert.AreEqual(imageUrl, expected.ImageUrl);

        var json = JsonConvert.SerializeObject(expected);
        var actual = JsonConvert.DeserializeObject<NewsEntry>(json) ?? throw new NullReferenceException();
        Assert.AreEqual(url, actual.Url);
        Assert.AreEqual(title, actual.Title);
        Assert.AreEqual(summary, actual.Summary);
        Assert.AreEqual(imageUrl, actual.ImageUrl);

        Assert.AreEqual(expected, actual);
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

        Assert.AreEqual(left, right);
        Assert.IsTrue(left.Equals(right));
        Assert.IsTrue(left == right);
        Assert.IsFalse(left != right);
        Assert.AreEqual(left.ToString(), right.ToString());
        Assert.AreEqual(left.GetHashCode(), right.GetHashCode());

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
                ? new Action<object, object>((a, b) => Assert.AreEqual(a, b))
                : (a, b) => Assert.AreNotEqual(a, b);
            assertEqual(left, other);
            Assert.AreEqual(isEqual, left.Equals(other));
            Assert.AreEqual(isEqual, left == other);
            Assert.AreEqual(!isEqual, left != other);
            assertEqual(left.GetHashCode(), other.GetHashCode());
            Assert.AreNotEqual(left.ToString(), other.ToString());
        }
    }

    [Test]
    public void TestEuqalityNullSafe()
    {
        var entry = new NewsEntry(null, null, null, null);
        NewsEntry? nullEntry = null;

        Assert.AreNotEqual(entry, nullEntry);
        Assert.AreEqual(entry, entry);
        Assert.AreEqual(nullEntry, nullEntry);

#pragma warning disable CS1718
        Assert.IsFalse(entry == nullEntry);
        Assert.IsFalse(nullEntry == entry);
        Assert.IsTrue(entry == entry);
        Assert.IsTrue(nullEntry == nullEntry);

        Assert.IsTrue(entry != nullEntry);
        Assert.IsTrue(nullEntry != entry);
        Assert.IsFalse(entry != entry);
        Assert.IsFalse(nullEntry != nullEntry);
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
        Assert.AreEqual(expectedTitle, newsEntry.Title);
        Assert.AreEqual(expectedSummary, newsEntry.Summary);
    }
}
