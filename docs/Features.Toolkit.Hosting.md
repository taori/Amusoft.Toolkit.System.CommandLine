# Features 

## Full sample

This sample uses 
- Amusoft.Toolkit.System.CommandLine
- Amusoft.Toolkit.System.CommandLine.Hosting
- Amusoft.Toolkit.System.CommandLine.Logging
- Amusoft.Toolkit.System.CommandLine.Generator

```cs

internal class Program
{
	static async Task<int> Main(string[] args)
	{
#if DEBUG
		while (true)
		{
			Console.WriteLine("Waiting for input");
			args = Console.ReadLine().Split(" ", StringSplitOptions.RemoveEmptyEntries);

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
}

[GenerateExecuteHandler]
public partial class Test2Command : Command
{
	private readonly ILogger<Test2Command> _logger;

	public Test2Command(ILogger<Test2Command> logger) : base("test2", "testcommand")
	{
		_logger = logger;
	}
}
```