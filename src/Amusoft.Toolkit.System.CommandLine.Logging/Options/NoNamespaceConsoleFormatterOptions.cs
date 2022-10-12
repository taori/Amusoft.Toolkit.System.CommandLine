using Microsoft.Extensions.Logging.Console;

namespace Amusoft.Toolkit.System.CommandLine.Logging.Options;

/// <summary>
/// Options for the built-in default console log formatter.
/// </summary>
public class NoNamespaceConsoleFormatterOptions : ConsoleFormatterOptions
{
	/// <summary>
	/// Determines when to use color when logging messages.
	/// </summary>
	public LoggerColorBehavior ColorBehavior { get; set; }


	/// <summary>
	/// When <see langword="true" />, the message is logged with the namespace
	/// </summary>
	public bool IncludeNamespace { get; set; }

	/// <summary>
	/// When <see langword="true" />, the entire message gets logged in a single line.
	/// </summary>
	public bool SingleLine { get; set; }
}