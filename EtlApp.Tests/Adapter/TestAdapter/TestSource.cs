using System;
using EtlApp.Domain.Config.Pipeline;
using EtlApp.Domain.Connection;
using EtlApp.Domain.Dto;
using EtlApp.Util.Observable;

namespace EtlApp.Tests.Adapter.TestAdapter;

public class TestSourceConfig : SourceConfig
{
    public required Observable<ReportData> Emitter;
}

public class TestSource : Observable<ReportData>, ISourceConnection
{
    class InnerObserver(TestSource source) : IObserver<ReportData>
    {
        public void OnCompleted()
        {
            source.Complete();
        }

        public void OnError(Exception error)
        {
            source.Error(error);
        }

        public void OnNext(ReportData value)
        {
            source.Next(value);
        }
    }

    public TestSource(TestSourceConfig config, PipelineContext context)
    {
        config.Emitter.Subscribe(new InnerObserver(this));
    }


    public void Fetch()
    {
        // Noop
    }
}