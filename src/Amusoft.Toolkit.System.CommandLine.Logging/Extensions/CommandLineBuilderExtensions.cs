using System;
using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Builder;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Amusoft.Toolkit.System.CommandLine.Logging.Extensions;

/// <summary>
/// Extensions for CommandLineBuilder
/// </summary>
public static class CommandLineBuilderExtensions
{
	/// <summary>
	/// Configures the application to override potentially configured logging sources for a given namespace to get a deeper insight into the application
	/// </summary>
	/// <param name="source"></param>
	/// <param name="logLevelOption">parameter where you should reference your instance of the LogLevelOption - ideally done through static declaration</param>
	/// <param name="logFilterOptionsReader">configures how to obtain an instance of <see cref="IOptionsMonitor{LoggerFilterOptions}"/></param>
	/// <param name="logNamespace">The namespace for which to apply the logging level to. If empty it will be applied to every logging namespace</param>
	/// <returns></returns>
	public static CommandLineBuilder UseRuntimeLogLevel(this CommandLineBuilder source, Func<Option<LogLevel>> logLevelOption, Func<BindingContext, IOptionsMonitor<LoggerFilterOptions>> logFilterOptionsReader, string? logNamespace = default)
	{
		source.AddMiddleware(async (context, next) =>
		{
			if (context.ParseResult.FindResultFor(logLevelOption()) is { } optionResult)
			{
				var logLevel = optionResult.GetValueOrDefault<LogLevel>();
				var logFilterOptions = logFilterOptionsReader(context.BindingContext);
			
				if (logNamespace is { })
				{
					logFilterOptions.CurrentValue.AddFilter(logNamespace, level => level >= logLevel);
				}
				else
				{
					logFilterOptions.CurrentValue.AddFilter(level => level >= logLevel);
				}
			}

			await next(context);
		});

		return source;
	}
}
