using System.Collections.Concurrent;
using System.Data.Common;

namespace EtlApp.Domain.Database
{
    public class DatabaseManager
    {
        private static readonly ConcurrentDictionary<string, DbConnection> ConnectionCache = new();

        public static void RegisterConnection(string key, DbConnection connection)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key cannot be null or empty.", nameof(key));

            ConnectionCache[key] = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public static DbConnection? GetConnection(string key)
        {
            return ConnectionCache.GetValueOrDefault(key);
        }
    }
}