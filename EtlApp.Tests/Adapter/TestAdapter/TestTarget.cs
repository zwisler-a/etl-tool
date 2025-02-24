using System;
using System.Collections.Generic;
using EtlApp.Domain.Config.Pipeline;
using EtlApp.Domain.Connection;
using EtlApp.Domain.Dto;
using EtlApp.Util.Observable;

namespace EtlApp.Tests.Adapter.TestAdapter;

public class TestTargetConfig : TargetConfig
{
    public required IObserver<ReportData> ReportObserver;
    public required List<UpdateStrategy> SupportedUpdateStrategies { get; set; }
}

public class TestTarget(TestTargetConfig config, PipelineContext context) : ITargetConnection
{
    public void OnCompleted()
    {
        config.ReportObserver.OnCompleted();
    }

    public void OnError(Exception error)
    {
        config.ReportObserver.OnError(error);
    }

    public void OnNext(ReportData value)
    {
        config.ReportObserver.OnNext(value);
    }

    public List<UpdateStrategy> GetSupportedUpdateStrategies()
    {
        return config.SupportedUpdateStrategies;
    }
}