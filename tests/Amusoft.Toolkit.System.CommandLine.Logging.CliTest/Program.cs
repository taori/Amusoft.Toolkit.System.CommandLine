﻿using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using Amusoft.Toolkit.System.CommandLine.Logging.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Amusoft.Toolkit.System.CommandLine.Logging.CliTest
{
	internal class Program
	{
		static async Task<int> Main(string[] args)
		{
			var serviceCollection = new ServiceCollection();
			serviceCollection
				.AddRuntimeLogLevel()
				.AddLogging(logging =>
			{
				logging.AddConsole(options =>
					{
						options.FormatterName = "nonamespace";
					})
					.AddNoNamespaceConsoleFormatter(options =>
					{
						options.IncludeNamespace = false;
					});
			});

			var command = new RootCommand();
			command.SetHandler(Handle);
			var commandLineBuilder = new CommandLineBuilder(command);
			using var sp = serviceCollection.BuildServiceProvider();

			return await commandLineBuilder
				.UseDefaults()
				.UseServiceProviderFallback(sp)
				.UseRuntimeLogLevel()
				.Build()
				.InvokeAsync(args);
		}

		private static void Handle(InvocationContext context)
		{
			// var logger = context.BindingContext.GetRequiredService<ILogger<Program>>();
			// var options = context.BindingContext.GetRequiredService<IOptions<RuntimeLogLevelOptions>>();

			// logger.LogInformation("Zeile 1 Zeile 1 Zeile 1 Zeile 1 Zeile 1 Zeile 1 Zeile 1 Zeile 1 Zeile 1 Zeile 1 Zeile 1 Zeile 1 Zeile 1 Zeile 1 Zeile 1 Zeile 1");
			// logger.LogInformation("Zeile 1 Zeile 1 Zeile 1 Zeile 1 Zeile 1 Zeile 1 Zeile 1 Zeile 1 Zeile 1 Zeile 1 Zeile 1 Zeile 1 Zeile 1 Zeile 1 Zeile 1 Zeile 1");
			// logger.LogInformation(
			// 	"""
			// Zeile 1
			// Zeile 2
			// Zeile 3
			// """);

			// foreach (var logLevel in Enum.GetValues<LogLevel>())
			// {
			// 	logger.Log(logLevel,
			// 		"""
			// 		Zeile 1
			// 		Zeile 2
			// 		Zeile 3
			// 		"""
			// 	);
			// 	// logger.Log(logLevel, "typ: {Level}", logLevel);
			// }
		}
	}
}