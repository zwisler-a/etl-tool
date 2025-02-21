using EtlApp.Domain.Config;

namespace EtlApp.Domain.Target
{
    public static class TargetConnectionFactory
    {
        private static readonly Dictionary<Type, Func<TargetConfig, ITargetConnection>> ConnectionCreators = new();

        public static ITargetConnection Create(TargetConfig config)
        {
            var configType = config.GetType();
            if (ConnectionCreators.TryGetValue(configType, out var creator))
            {
                return creator(config);
            }

            throw new ArgumentException($"Invalid Target type: {configType.Name}", nameof(config));
        }

        public static void Register<T>(Func<T, ITargetConnection> creator) where T : TargetConfig
        {
            ConnectionCreators[typeof(T)] = config => creator((T)config);
        }
    }
}