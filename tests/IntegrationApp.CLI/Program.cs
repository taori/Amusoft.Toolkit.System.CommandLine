using Microsoft.Extensions.Logging;
using System.CommandLine;
using System.CommandLine.Parsing;
using Amusoft.Toolkit.System.CommandLine.Attributes;
using Amusoft.Toolkit.System.CommandLine.Hosting;
using Amusoft.Toolkit.System.CommandLine.Logging.Extensions;
using Amusoft.Toolkit.System.CommandLine.Logging.Parameters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationApp.CLI;

internal class Program
{
	static async Task<int> Main(string[] args)
	{
#if DEBUG
		while (true)
		{
			Console.WriteLine("Waiting for input");
			var line = Console.ReadLine();

			if (string.IsNullOrEmpty(line))
				return 0;

			args = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);

			if (args.Length == 0)
				return 0;

			await RunApplicationAsync(args);
		}
#else
			return await RunApplicationAsync(args);
#endif
	}

	private static async Task<int> RunApplicationAsync(string[] args)
	{
		return await HostedRootCommandBuilder.ForCommand<ApplicationRootCommand>()
#if DEBUG
			.UseDefaultConfiguration("Development")
#else
			.UseDefaultConfiguration("Production")
#endif
			.UseDefaultCommandLineBuilder()
			.ConfigureServices((configuration, collection) =>
			{
				collection
					.AddCommandHierarchy(builder =>
					{
						builder
							.AddMapping(typeof(ApplicationRootCommand), typeof(TestCommand))
							.AddMapping(typeof(ApplicationRootCommand), typeof(Test2Command))
							;
					})
					.AddRuntimeLogLevel(options =>
					{
						options.Namespace = configuration.GetValue<string>("Logging:RunTimeLogLevel:Namespace");
						options.DefaultLevel = configuration.GetValue<LogLevel>("Logging:RunTimeLogLevel:DefaultLevel");
						options.LogLevelOption = GlobalParameters.Logging;
					})
					.AddLogging(logging =>
					{
						logging.AddConfiguration(configuration.GetSection("Logging"))
							.AddNoNamespaceConsoleFormatter()
							.AddConsole();
					});

			})
			.Build((context, builder) =>
			{
				builder
					.UseRuntimeLogLevel();
			})
			.InvokeAsync(args);
	}
}

public class ApplicationRootCommand : RootCommand
{
	public ApplicationRootCommand()
	{
		AddGlobalOption(GlobalParameters.Logging);
	}
}

public static class GlobalParameters
{
	public static readonly LogLevelOption Logging = new(LogLevel.Information);
}

public class TestCommand : Command
{
	private readonly ILogger<TestCommand> _logger;

	public TestCommand(ILogger<TestCommand> logger) : base("test", "testcommand")
	{
		_logger = logger;

		this.SetHandler(context =>
		{
			_logger.LogDebug("Debug from test");
			_logger.LogInformation("Information from test");
			_logger.LogWarning("Warning from test");
			_logger.LogError("Error from test");
		});
	}
}

public class Test2Command : Command
{
	private readonly ILogger<Test2Command> _logger;

	public Test2Command(ILogger<Test2Command> logger) : base("test2", "testcommand")
	{
		_logger = logger;

		this.SetHandler(context =>
		{
			_logger.LogDebug("Debug from test2");
			_logger.LogInformation("Information from test2");
			_logger.LogWarning("Warning from test2");
			_logger.LogError("Error from test2");
		});
	}
}