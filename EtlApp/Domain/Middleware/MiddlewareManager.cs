using System.Collections.Concurrent;
using System.Data.Common;

namespace EtlApp.Domain.Middleware
{
    public class MiddlewareManager
    {
        private readonly ConcurrentDictionary<string, IMiddleware> _middlewares = new();

        public void Register(string key, IMiddleware connection)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key cannot be null or empty.", nameof(key));

            _middlewares[key] = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public IMiddleware? Get(string key)
        {
            if (_middlewares.ContainsKey(key))
            {
                return _middlewares.GetValueOrDefault(key);
            }

            throw new KeyNotFoundException(key);
        }
    }
}