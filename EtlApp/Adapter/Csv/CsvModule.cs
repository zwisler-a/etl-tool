﻿using System.Data.Common;
using EtlApp.Domain.Config;
using EtlApp.Domain.Database;
using EtlApp.Domain.Module;
using EtlApp.Domain.Source;
using EtlApp.Domain.Target;

namespace EtlApp.Adapter.Csv;

public class CsvModule : Module
{
    public void RegisterSourceConnection(SourceConnectionFactory factory)
    {
        SourceConfigConverter.Register<CsvSourceConfig>("csv");
        factory.Register((CsvSourceConfig config) => new CsvSourceConnection(config));
    }

    public void RegisterTargetConnection(TargetConnectionFactory factory)
    {
        TargetConfigConverter.Register<CsvTargetConfig>("csv");
        factory.Register((CsvTargetConfig config) => new CsvTargetConnection(config));
    }

    public void RegisterConnections(DatabaseManager databaseManager)
    {
        // No Db
    }
}