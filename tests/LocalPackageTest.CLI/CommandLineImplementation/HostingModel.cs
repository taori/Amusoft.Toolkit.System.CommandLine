using LocalPackageTest.CLI.Commands;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.Parsing;
using Amusoft.Toolkit.System.CommandLine.Logging.Extensions;
using Amusoft.Toolkit.System.CommandLine.Logging.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LocalPackageTest.CLI.CommandLineImplementation;

public class HostingModel : ImplementationBase
{
	public override Task<int> Execute(string[] args)
	{
		var serviceCollection = new ServiceCollection();
		ServiceSetup.Register(serviceCollection);
		using var serviceProvider = serviceCollection.BuildServiceProvider();
		var command = new ApplicationRootCommand();
		var commandLineBuilder = new CommandLineBuilder(command);

		return RunApplicationAsync(args, commandLineBuilder, serviceProvider);
	}

	private static Task<int> RunApplicationAsync(string[] args, CommandLineBuilder commandLineBuilder, ServiceProvider serviceProvider)
	{
		return commandLineBuilder
			.UseHost(_ => Host.CreateDefaultBuilder(args), builder =>
			{
				builder.UseContentRoot(Path.GetDirectoryName(typeof(Program).Assembly.Location));
				builder.UseConsoleLifetime();
				builder.ConfigureServices((context, services) =>
				{
					ServiceSetup.Register(services);

					services.AddLogging(logging =>
					{
						logging.AddConsole(options =>
							{
								options.FormatterName = "nonamespace";
							})
							.AddNoNamespaceConsoleFormatter();
					});
				});
			})
			.UseDefaults()
			.UseServiceProviderFallback(serviceProvider)
			.UseRuntimeLogLevel()
			.Build()
			.InvokeAsync(args);
	}
}

public static class ServiceSetup
{
	public static void Register(IServiceCollection serviceCollection)
	{
		serviceCollection.AddRuntimeLogLevel(options =>
		{
			options.Namespace = "LocalPackageTest.CLI";
		});
	}
}