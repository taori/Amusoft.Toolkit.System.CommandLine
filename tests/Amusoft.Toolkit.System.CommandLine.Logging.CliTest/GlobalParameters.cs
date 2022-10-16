using Amusoft.Toolkit.System.CommandLine.Logging.Parameters;
using Microsoft.Extensions.Logging;

namespace Amusoft.Toolkit.System.CommandLine.Logging.CliTest;

public static class GlobalParameters
{
	public static LogLevelOption Logging = new(LogLevel.Information);
}