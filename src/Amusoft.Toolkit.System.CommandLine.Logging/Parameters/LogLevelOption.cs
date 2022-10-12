using System.CommandLine;
using Microsoft.Extensions.Logging;

namespace Amusoft.Toolkit.System.CommandLine.Logging.Parameters;

/// <summary>
/// Log level option using the enum Types of Microsoft.Extensions.Logging
/// </summary>
public class LogLevelOption : Option<LogLevel>
{
    /// <summary>
    /// Constructor using the default LogLevel
    /// </summary>
    /// <param name="logLevelDefault">default log level</param>
    public LogLevelOption(LogLevel logLevelDefault) : base("--logLevel", () => logLevelDefault, "log level for custom application code")
    {
    }
}