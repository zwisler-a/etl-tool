using System.Collections.Concurrent;
using System.Data.Common;

namespace EtlApp.Domain.Connection
{
    public class DatabaseManager
    {
        private readonly ConcurrentDictionary<string, DbConnection> _connectionCache = new();

        public void RegisterConnection(string key, DbConnection connection)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key cannot be null or empty.", nameof(key));

            _connectionCache[key] = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public DbConnection? GetConnection(string key)
        {
            if (_connectionCache.ContainsKey(key))
            {
                return _connectionCache.GetValueOrDefault(key);
            }

            throw new KeyNotFoundException(key);
        }
    }
}