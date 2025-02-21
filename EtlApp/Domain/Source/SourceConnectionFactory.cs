using EtlApp.Domain.Config;

namespace EtlApp.Domain.Source
{
    public static class SourceConnectionFactory
    {
        private static readonly Dictionary<Type, Func<SourceConfig, ISourceConnection>> ConnectionCreators = new();

        public static ISourceConnection Create(SourceConfig config)
        {
            var configType = config.GetType();
            if (ConnectionCreators.TryGetValue(configType, out var creator))
            {
                return creator(config);
            }

            throw new ArgumentException($"Invalid source type: {configType.Name}", nameof(config));
        }

        public static void Register<T>(Func<T, ISourceConnection> creator) where T : SourceConfig
        {
            ConnectionCreators[typeof(T)] = config => creator((T)config);
        }
    }
}