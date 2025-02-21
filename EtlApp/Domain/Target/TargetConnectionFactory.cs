using EtlApp.Domain.Config;

namespace EtlApp.Domain.Target
{
    public class TargetConnectionFactory
    {
        private readonly Dictionary<Type, Func<TargetConfig, ITargetConnection>> _connectionCreators = new();

        public ITargetConnection Create(TargetConfig config)
        {
            var configType = config.GetType();
            if (_connectionCreators.TryGetValue(configType, out var creator))
            {
                return creator(config);
            }

            throw new ArgumentException($"Invalid Target type: {configType.Name}", nameof(config));
        }

        public void Register<T>(Func<T, ITargetConnection> creator) where T : TargetConfig
        {
            _connectionCreators[typeof(T)] = config => creator((T)config);
        }
    }
}