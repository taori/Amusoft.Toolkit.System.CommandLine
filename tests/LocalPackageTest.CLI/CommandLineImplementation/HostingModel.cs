using LocalPackageTest.CLI.Commands;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.Parsing;
using LocalPackageTest.CLI.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LocalPackageTest.CLI.CommandLineImplementation;

public class HostingModel : ImplementationBase
{
	public override Task<int> Execute(string[] args)
	{
		var command = new ApplicationRootCommand();
		var commandLineBuilder = new CommandLineBuilder(command);

		return RunApplicationAsync(args, commandLineBuilder);
	}

	private static Task<int> RunApplicationAsync(string[] args, CommandLineBuilder commandLineBuilder)
	{
		return commandLineBuilder
			.UseHost(_ => Host.CreateDefaultBuilder(args), builder =>
			{
				builder.UseContentRoot(Path.GetDirectoryName(typeof(Program).Assembly.Location));
				builder.UseConsoleLifetime();
				builder.ConfigureServices((context, services) => { services.AddLogging(logging => logging.AddConsole()); });
			})
			.UseDefaults()
			.UseRuntimeLogLevel("LocalPackageTest.CLI")
			.Build()
			.InvokeAsync(args);
	}
}