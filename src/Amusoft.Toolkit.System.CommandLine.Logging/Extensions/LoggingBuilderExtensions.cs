using System;
using Amusoft.Toolkit.System.CommandLine.Logging.Formatters;
using Amusoft.Toolkit.System.CommandLine.Logging.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Amusoft.Toolkit.System.CommandLine.Logging.Extensions;

/// <summary>
/// Extension methods targeted for <see cref="ILoggingBuilder"/>
/// </summary>
public static class LoggingBuilderExtensions
{
	/// <summary>
	/// Adds a formatter named "nonamespace"
	/// </summary>
	/// <param name="source"></param>
	/// <param name="configure"></param>
	/// <returns></returns>
	public static ILoggingBuilder AddNoNamespaceConsoleFormatter(this ILoggingBuilder source, Action<NoNamespaceConsoleFormatterOptions>? configure = default)
	{
		source.AddConsoleFormatter<NoNamespaceConsoleFormatter, NoNamespaceConsoleFormatterOptions>();

		if (configure is not null)
			source.Services.Configure(configure);

		return source;
	}
}