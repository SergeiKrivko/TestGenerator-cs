using Serilog;

namespace Shared.Utils;

public class Logger
{
    public string? Name { get; }
    private readonly ILogger _logger;

    public Logger(string name, ILogger logger)
    {
        Name = name;
        _logger = logger;
    }

    public void Debug(string message) => _logger.Debug($"({Name}) {message}");
    public void Info(string message) => _logger.Information($"({Name}) {message}");
    public void Warning(string message) => _logger.Warning($"({Name}) {message}");
    public void Error(string message) => _logger.Error($"({Name}) {message}");
    public void Fatal(string message) => _logger.Fatal($"({Name}) {message}");
}