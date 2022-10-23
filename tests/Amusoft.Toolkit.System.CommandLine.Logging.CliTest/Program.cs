using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using Amusoft.Toolkit.System.CommandLine.Attributes;
using Amusoft.Toolkit.System.CommandLine.CommandModel;
using Amusoft.Toolkit.System.CommandLine.Hosting;
using Amusoft.Toolkit.System.CommandLine.Logging.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Amusoft.Toolkit.System.CommandLine.Logging.CliTest;

internal class Program
{
	static async Task<int> Main(string[] args)
	{
		return await HostedRootCommandBuilder.Create()
			.UseDefaultConfiguration()
			.UseDefaultCommandLineBuilder()
			.ConfigureServices((configuration, collection) =>
			{
				collection
					.AddCommandHierarchy(CommandHierarchyBuilder.FromArray(new[,]
					{
						{typeof(ApplicationRootCommand), typeof(TestCommand)},
						{typeof(ApplicationRootCommand), typeof(Test2Command)},
					}))
					.AddRuntimeLogLevel(options =>
					{
						options.Namespace = "Amusoft.Toolkit.System.CommandLine.Logging.CliTest";
						options.DefaultLevel = LogLevel.Information;
						options.LogLevelOption = GlobalParameters.Logging;
					})
					.AddLogging(logging =>
					{
						logging
							.AddNoNamespaceConsoleFormatter()
							.AddConsole(options =>
							{
								options.FormatterName = "nonamespace";
							});
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

#pragma warning disable ATSCG001

public class ApplicationRootCommand : RootCommand
{
	public ApplicationRootCommand()
	{
		AddOption(GlobalParameters.Logging);
	}
}

[GenerateExecuteHandler]
public partial class TestCommand : Command
{
	private readonly ILogger<TestCommand> _logger;

	public TestCommand(ILogger<TestCommand> logger) : base("test", "testcommand")
	{
		_logger = logger;
	}

	public Task ExecuteAsync(InvocationContext context)
	{
		_logger.LogInformation("Command {Name} called", nameof(TestCommand));
		return Task.CompletedTask;
	}
}

[GenerateExecuteHandler]
public partial class Test2Command : Command
{
	private readonly ILogger<Test2Command> _logger;

	public Test2Command(ILogger<Test2Command> logger) : base("test2", "testcommand")
	{
		_logger = logger;
	}

	public Task ExecuteAsync(InvocationContext context)
	{
		_logger.LogInformation("Command {Name} called", nameof(Test2Command));
		return Task.CompletedTask;
	}
}