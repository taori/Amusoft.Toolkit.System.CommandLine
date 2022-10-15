using System;
using System.CommandLine.Binding;
using System.CommandLine.Builder;
using Amusoft.Toolkit.System.CommandLine.Extensions;
using Amusoft.Toolkit.System.CommandLine.Logging.Options;
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
	/// <param name="serviceProvider"> fallback service provider to feed services into the middleware</param>
	/// <returns></returns>
	public static CommandLineBuilder UseRuntimeLogLevel(this CommandLineBuilder source, IServiceProvider? serviceProvider = default)
	{
		source.AddMiddleware(async (context, next) =>
		{
			var runtimeOptions = context.BindingContext.GetRequiredServiceWithFallback<IOptions<RuntimeLogLevelOptions>>(serviceProvider);
			if (runtimeOptions.Value.LogLevelOption is {} minLevelOption && context.ParseResult.FindResultFor(minLevelOption) is { } optionResult)
			{
				var logLevel = optionResult.GetValueOrDefault<LogLevel>();
				var logFilterOptions = context.BindingContext.GetRequiredServiceWithFallback<IOptionsMonitor<LoggerFilterOptions>>(serviceProvider);
			
				if (runtimeOptions.Value.Namespace is { })
				{
					logFilterOptions.CurrentValue.AddFilter(runtimeOptions.Value.Namespace, level => level >= logLevel);
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

	/// <summary>
	/// Adds an external serviceProvider to the BindingContext
	/// </summary>
	/// <param name="source"></param>
	/// <param name="serviceProvider">service provider instance to fall back to</param>
	/// <returns></returns>
	public static CommandLineBuilder UseServiceProviderFallback(this CommandLineBuilder source, IServiceProvider serviceProvider)
	{
		source.AddMiddleware(context =>
		{
			context.BindingContext.AddService(typeof(IServiceProvider), _ => serviceProvider);
		});

		return source;
	}
}
