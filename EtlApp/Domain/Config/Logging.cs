using Microsoft.Extensions.Logging;

namespace EtlApp.Domain.Config;

public class Logging
{
    private static LogLevel _logLevel = LogLevel.Information;
    private static ILoggerFactory? _loggerFactory;

    public static ILoggerFactory LoggerFactory => _loggerFactory ??= CreateLoggerFactory();

    private static ILoggerFactory CreateLoggerFactory()
    {
        return Microsoft.Extensions.Logging.LoggerFactory.Create(builder =>
        {
            builder.AddSimpleConsole(options =>
            {
                options.IncludeScopes = true;
                options.SingleLine = true;
                options.TimestampFormat = "HH:mm:ss ";
            });
            builder.SetMinimumLevel(_logLevel);
        });
    }

    public static void SetLoggingLevel(LogLevel logLevel)
    {
        _logLevel = logLevel;
        _loggerFactory = CreateLoggerFactory();
    }
}