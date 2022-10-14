using System.CommandLine;
using Microsoft.Extensions.Logging;

namespace Amusoft.Toolkit.System.CommandLine.Logging.Options;

/// <summary>
/// Used to configure options for runtime log levels
/// </summary>
public class RuntimeLogLevelOptions
{
	/// <summary>
	/// Default logging level
	/// </summary>
	public LogLevel DefaultLevel { get; set; }

	/// <summary>
	/// Namespace to apply runtime logging to
	/// </summary>
	public string? Namespace { get; set; }

	/// <summary>
	/// Reference to the global logLevel option - this must be set by the library user
	/// </summary>
	public Option<LogLevel>? LogLevelOption { get; set; }
}