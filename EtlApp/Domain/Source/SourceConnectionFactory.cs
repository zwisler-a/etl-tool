using EtlApp.Domain.Config;
using Microsoft.Extensions.Logging;

namespace EtlApp.Domain.Source
{
    public class SourceConnectionFactory
    {
        private readonly Dictionary<Type, Func<SourceConfig, ISourceConnection>> _connectionCreators = new();

        private readonly ILogger _logger = LoggerFactory.Create(builder => builder.AddConsole())
            .CreateLogger<SourceConnectionFactory>();


        public ISourceConnection Create(SourceConfig config)
        {
            var configType = config.GetType();
            if (_connectionCreators.TryGetValue(configType, out var creator))
            {
                _logger.LogDebug("Creating source of type {}", configType.Name);
                return creator(config);
            }

            throw new ArgumentException($"Invalid source type: {configType.Name}", nameof(config));
        }

        public void Register<T>(Func<T, ISourceConnection> creator) where T : SourceConfig
        {
            _logger.LogDebug("Registering {}", typeof(T).Name);
            _connectionCreators[typeof(T)] = config => creator((T)config);
        }
    }
}