using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.CommandLine.Builder;
using LocalPackageTest.CLI.Commands;

namespace LocalPackageTest.CLI.Extensions;

public static class CommandLineBuilderExtensions
{
	public static CommandLineBuilder UseRuntimeLogLevel(this CommandLineBuilder source, string? logNamespace = default)
	{
		source.AddMiddleware(async (context, next) =>
		{
			if (context.ParseResult.FindResultFor(ApplicationRootCommand.LogLevelOption) is { } optionResult)
			{
				var logLevel = optionResult.GetValueOrDefault<LogLevel>();
				var host = context.BindingContext.GetRequiredService<IHost>();
				var logFilterOptions = host.Services.GetRequiredService<IOptionsMonitor<LoggerFilterOptions>>();

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
