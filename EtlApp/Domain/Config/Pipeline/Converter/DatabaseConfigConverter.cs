using System.Text.Json;
using System.Text.Json.Serialization;
using EtlApp.Util;

namespace EtlApp.Domain.Config;
public class DatabaseConfigConverter: JsonDerivedTypeConverter<DatabaseConfig>;