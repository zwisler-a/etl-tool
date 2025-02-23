namespace EtlApp.Util
{
    public class Factory<T1, T2, T3>
        where T1 : class
        where T2 : class
        where T3 : class
    {
        
        private readonly Dictionary<Type, Func<T1, T2, T3>> _creators =
            new();

        public T3 Create(T1 config, T2 context)
        {
            var configType = config.GetType();
            if (_creators.TryGetValue(configType, out var creator))
            {
                return creator(config, context);
            }

            throw new ArgumentException($"Invalid type: {configType.Name}", nameof(config));
        }

        public void Register<T>(Func<T, T2, T3> creator) where T : T1
        {
            _creators[typeof(T)] = (config, context) => creator((T)config, context);
        }
    }
}