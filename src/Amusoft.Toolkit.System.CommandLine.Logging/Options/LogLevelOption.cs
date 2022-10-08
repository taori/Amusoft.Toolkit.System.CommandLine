using System.CommandLine;
using Microsoft.Extensions.Logging;

namespace Amusoft.Toolkit.System.CommandLine.Logging.Options;

public class LogLevelOption : Option<LogLevel>
{
	public LogLevelOption(LogLevel logLevelDefault) : base("--logLevel", () => logLevelDefault, "log level for custom application code")
	{
	}
}