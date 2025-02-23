using CsvHelper;
using EtlApp.Util;
using NUnit.Framework;

namespace EtlApp.Tests.Util;

[TestFixture]
[TestOf(typeof(Factory))]
public class FactoryTest
{
    class Config;

    class SubConfigA : Config;

    class SubConfigB : Config;

    class Context
    {
    }

    class Object
    {
    }

    class SubObjectA(SubConfigA c) : Object
    {
    }

    class SubObjectB(SubConfigB c) : Object
    {
    }

    [Test]
    public void TestFactory()
    {
        var factory = new Factory<Config, Context, Object>();
        factory.Register<SubConfigA>((config, context) => new SubObjectA(config));
        factory.Register<SubConfigB>((config, context) => new SubObjectB(config));

        var shouldBeA = factory.Create(new SubConfigA(), new Context());
        var shouldBeB = factory.Create(new SubConfigB(), new Context());

        Assert.That(shouldBeA, Is.TypeOf<SubObjectA>());
        Assert.That(shouldBeB, Is.TypeOf<SubObjectB>());
    }
}