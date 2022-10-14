using System;
using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Builder;
using System.Reflection;
using Amusoft.Toolkit.System.CommandLine.Logging.Parameters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Amusoft.Toolkit.System.CommandLine.Logging.Extensions;

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
}

/// <summary>
/// Extensions for CommandLineBuilder
/// </summary>
public static class CommandLineBuilderExtensions
{
	/// <summary>
	/// Adds runtime log level support
	/// </summary>
	/// <param name="source"></param>
	/// <param name="configure"></param>
	/// <returns></returns>
	public static IServiceCollection AddRuntimeLogLevel(this IServiceCollection source, Action<RuntimeLogLevelOptions>? configure = default)
	{
		if (configure is null)
		{
			configure = _ => { };
		}
		
		source.TryAddSingleton(new RuntimeLogLevelOptions()
		{
			Namespace = Assembly.GetEntryAssembly().GetName().Name,
			DefaultLevel = LogLevel.Information,
		});
		source.Configure(configure);

		return source;
	}

	/// <summary>
	/// Configures the application to override potentially configured logging sources for a given namespace to get a deeper insight into the application
	/// </summary>
	/// <param name="source"></param>
	/// <param name="logLevelOption">parameter where you should reference your instance of the LogLevelOption - ideally done through static declaration</param>
	/// <param name="logFilterOptionsReader">configures how to obtain an instance of <see cref="IOptionsMonitor{LoggerFilterOptions}"/></param>
	/// <param name="logNamespace">The namespace for which to apply the logging level to. If empty it will be applied to every logging namespace</param>
	/// <returns></returns>
	public static CommandLineBuilder UseRuntimeLogLevel(this CommandLineBuilder source, Func<Option<LogLevel>> logLevelOption, Func<BindingContext, IOptionsMonitor<LoggerFilterOptions>> logFilterOptionsReader, string? logNamespace = default, IServiceProvider sp = default)
	{
		source.AddMiddleware(async (context, next) =>
		{
			var a = context.BindingContext.GetService<IOptionsMonitor<LoggerFilterOptions>>()
			        ?? sp?.GetService<IOptionsMonitor<LoggerFilterOptions>>();

			var b = context.BindingContext.GetRequiredService<IOptionsMonitor<LoggerFilterOptions>>();
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
