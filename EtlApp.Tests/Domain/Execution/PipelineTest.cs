using System;
using System.Collections.Generic;
using System.Data;
using EtlApp.Adapter.BuildIn;
using EtlApp.Adapter.BuildIn.Middlewares;
using EtlApp.Domain.Config.Pipeline;
using EtlApp.Domain.Dto;
using EtlApp.Domain.Execution;
using EtlApp.Domain.Module;
using EtlApp.Tests.Adapter.TestAdapter;
using EtlApp.Util.Observable;
using NUnit.Framework;
using ConfigurationManager = EtlApp.Domain.Config.ConfigurationManager;

namespace EtlApp.Tests.Domain.Execution;

[TestFixture]
[TestOf(typeof(Pipeline))]
public class PipelineTest
{
    [Test]
    public void SimplePipelineTest()
    {
        ConfigurationManager.ConfigFilePath = "../../../TestConfig/application_config.json";
        var moduleRegistry = new ModuleRegistry();
        moduleRegistry.RegisterModule(new TestModule());
        var builder = new PipelineBuilder(moduleRegistry);
        var emitter = new Subject<ReportData>();
        var counter = 0;
        ReportData targetReport = null;
        var observer = new LambdaObserver<ReportData>(
            data =>
            {
                counter++;
                targetReport = data;
            }
        );
        var testSource = new TestSourceConfig
        {
            Type = "test",
            Emitter = emitter
        };
        var testTarget = new TestTargetConfig
        {
            Type = "test",
            UpdateStrategy = UpdateStrategy.Append,
            ReportObserver = observer,
            SupportedUpdateStrategies = [UpdateStrategy.Append]
        };
        var pipeline = builder.Build(
            [testSource],
            [testTarget],
            [],
            [],
            []
        );
        pipeline.Run();
        var mockData = new ReportData(new DataTable(), new Dictionary<string, ColumnMappingConfig>());
        emitter.Next(mockData);
        Assert.That(counter, Is.EqualTo(1));
        Assert.That(targetReport!, Is.EqualTo(mockData));
    }

    [Test]
    public void PipelineWithBuildInMiddlewareTest()
    {
        ConfigurationManager.ConfigFilePath = "../../../TestConfig/application_config.json";
        var moduleRegistry = new ModuleRegistry();
        moduleRegistry.RegisterModule(new TestModule());
        moduleRegistry.RegisterModule(new BuildInModule());
        var builder = new PipelineBuilder(moduleRegistry);
        var emitter = new Subject<ReportData>();
        var counter = 0;
        ReportData targetReport = null;
        var observer = new LambdaObserver<ReportData>(
            data =>
            {
                counter++;
                targetReport = data;
            }
        );
        var testSource = new TestSourceConfig
        {
            Type = "test",
            Emitter = emitter
        };
        var testTarget = new TestTargetConfig
        {
            Type = "test",
            UpdateStrategy = UpdateStrategy.Append,
            ReportObserver = observer,
            SupportedUpdateStrategies = [UpdateStrategy.Append]
        };
        var pipeline = builder.Build(
            [testSource],
            [testTarget],
            [],
            [],
            [
                new ApplyTransformMiddlewareConfig { Type = "apply_transformers" },
                new DetectTypeMiddlewareConfig { Type = "type_inference" },
                new CastTypeMiddlewareConfig { Type = "type_cast" },
                new ValidateTypeMiddlewareConfig { Type = "type_validation" }
            ]
        );
        pipeline.Run();
        var mockData = new ReportData(new DataTable(), new Dictionary<string, ColumnMappingConfig>());
        emitter.Next(mockData);
        emitter.Complete();
        Assert.That(counter, Is.EqualTo(1));
    }
    
    [Test]
    public void PipelineWithErrorTest()
    {
        ConfigurationManager.ConfigFilePath = "../../../TestConfig/application_config.json";
        var moduleRegistry = new ModuleRegistry();
        moduleRegistry.RegisterModule(new TestModule());
        moduleRegistry.RegisterModule(new BuildInModule());
        var builder = new PipelineBuilder(moduleRegistry);
        var emitter = new Subject<ReportData>();
        var counter = 0;
        ReportData targetReport = null;
        var observer = new LambdaObserver<ReportData>(
            data =>
            {
                counter++;
                targetReport = data;
            }
        );
        var testSource = new TestSourceConfig
        {
            Type = "test",
            Emitter = emitter
        };
        var testTarget = new TestTargetConfig
        {
            Type = "test",
            UpdateStrategy = UpdateStrategy.Append,
            ReportObserver = observer,
            SupportedUpdateStrategies = [UpdateStrategy.Append]
        };
        var pipeline = builder.Build(
            [testSource],
            [testTarget],
            [],
            [],
            [
                new ApplyTransformMiddlewareConfig { Type = "apply_transformers" },
                new DetectTypeMiddlewareConfig { Type = "type_inference" },
                new CastTypeMiddlewareConfig { Type = "type_cast" },
                new ValidateTypeMiddlewareConfig { Type = "type_validation" }
            ]
        );
        pipeline.Run();
        var mockData = new ReportData(new DataTable(), new Dictionary<string, ColumnMappingConfig>());
        emitter.Next(mockData);
        emitter.Error(new Exception("Test"));
    }
}