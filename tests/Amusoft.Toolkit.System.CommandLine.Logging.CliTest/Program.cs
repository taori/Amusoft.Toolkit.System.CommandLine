using Amusoft.Toolkit.System.CommandLine.Logging.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Amusoft.Toolkit.System.CommandLine.Logging.CliTest
{
	internal class Program
	{
		static void Main(string[] args)
		{
			var serviceCollection = new ServiceCollection();
			serviceCollection.AddLogging(logging =>
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

			using var sp = serviceCollection.BuildServiceProvider();
			var logger = sp.GetRequiredService<ILogger<Program>>();

			foreach (var logLevel in Enum.GetValues<LogLevel>())
			{
				// logger.Log(logLevel, "typ: {Level}", logLevel);
			}

			// logger.LogInformation("Zeile 1 Zeile 1 Zeile 1 Zeile 1 Zeile 1 Zeile 1 Zeile 1 Zeile 1 Zeile 1 Zeile 1 Zeile 1 Zeile 1 Zeile 1 Zeile 1 Zeile 1 Zeile 1");
			// logger.LogInformation("Zeile 1 Zeile 1 Zeile 1 Zeile 1 Zeile 1 Zeile 1 Zeile 1 Zeile 1 Zeile 1 Zeile 1 Zeile 1 Zeile 1 Zeile 1 Zeile 1 Zeile 1 Zeile 1");
			logger.LogInformation(
			"""
			Zeile 1
			Zeile 2
			Zeile 3
			""");

			foreach (var logLevel in Enum.GetValues<LogLevel>())
			{
				logger.Log(logLevel,
					"""
					Zeile 1
					Zeile 2
					Zeile 3
					"""
					);
				// logger.Log(logLevel, "typ: {Level}", logLevel);
			}
		}
	}
}