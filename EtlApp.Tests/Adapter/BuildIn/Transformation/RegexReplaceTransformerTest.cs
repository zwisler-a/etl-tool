using EtlApp.Adapter.BuildIn.Transformation;
using NUnit.Framework;

namespace EtlApp.Tests.Adapter.BuildIn.Transformation;

[TestFixture]
[TestOf(typeof(RegexReplaceTransformer))]
public class RegexReplaceTransformerTest
{
    [Test]
    public void TestReplacement()
    {
        var transformer = new RegexReplaceTransformer(new RegexReplaceTransformerConfig
        {
            Name = "test",
            Type = "test",
            SelectRegex = ".+",
            ReplaceRegex = "1",
        });
        Assert.That(transformer.Transform("Hello"), Is.EqualTo("1"));
    }
    
    [Test]
    public void TestCaptureGroups()
    {
        var transformer = new RegexReplaceTransformer(new RegexReplaceTransformerConfig
        {
            Name = "test",
            Type = "test",
            SelectRegex = "(\\d+)a(\\d+)",
            ReplaceRegex = "$2b$1",
        });
        Assert.That(transformer.Transform("42a1337"), Is.EqualTo("1337b42"));
    }
}