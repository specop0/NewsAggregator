using System;
using System.IO;
using NUnit.Framework;

namespace NewsAggregator.Tests;

[TestFixture]
public class TestsBase
{
    [SetUp]
    public void SetUpNUnit()
    {
        this.SetUp();
    }

    protected virtual void SetUp() { }

    protected string GetUniqueName(string? prefix = null)
    {
        if (string.IsNullOrEmpty(prefix))
        {
            prefix = string.Empty;
        }
        return $"{prefix}{Guid.NewGuid()}";
    }

    protected string GetResource(params string[] path)
    {
        var name = $"{this.GetType().Namespace}.Resources.{string.Join(".", path)}";
        var stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(name) ?? throw new NullReferenceException();
        var reader = new StreamReader(stream);
        var content = reader.ReadToEnd();
        return content;
    }
}
