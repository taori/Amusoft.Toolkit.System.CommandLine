using System.CommandLine;
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
				.AddOptions()
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

			var lfo = sp.GetRequiredService<IOptionsMonitor<LoggerFilterOptions>>();
			commandLineBuilder.AddMiddleware(context =>
			{
				context.BindingContext.AddService(typeof(ILogger<>), f => sp.GetRequiredService(typeof(ILogger<>)));
				context.BindingContext.AddService(typeof(IOptions<>), f => sp.GetRequiredService(typeof(IOptions<>)));
				context.BindingContext.AddService(typeof(IOptions<>), f => sp.GetRequiredService(typeof(OptionsManager<>)));
				context.BindingContext.AddService(typeof(IOptionsSnapshot<>),f => sp.GetRequiredService(typeof(IOptionsSnapshot<>)));
				context.BindingContext.AddService(typeof(IOptionsMonitor<>), f => sp.GetRequiredService(typeof(IOptionsMonitor<>)));
				context.BindingContext.AddService(typeof(IOptionsFactory<>), f => sp.GetRequiredService(typeof(IOptionsFactory<>)));
				context.BindingContext.AddService(typeof(IOptionsMonitorCache<>), f => sp.GetRequiredService(typeof(IOptionsMonitorCache<>)));
			});

			return await commandLineBuilder
				.UseDefaults()
				.UseRuntimeLogLevel(null, null, sp: sp)
				.Build()
				.InvokeAsync(args);
		}

		private static void Handle(InvocationContext context)
		{
			var logger = context.BindingContext.GetRequiredService<ILogger<Program>>();
			var options = context.BindingContext.GetRequiredService<IOptions<RuntimeLogLevelOptions>>();

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