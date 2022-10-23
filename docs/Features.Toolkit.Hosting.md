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
		return await HostedRootCommandBuilder.Create()
#if DEBUG
			.UseDefaultConfiguration("Development")
#else
			.UseDefaultConfiguration("Production")
#endif
			.UseDefaultCommandLineBuilder()
			.ConfigureServices((configuration, collection) =>
			{
				collection
					.AddCommandHierarchy(GeneratedHierarchy.ApplicationRootCommand)
					.AddRuntimeLogLevel(options =>
					{
						options.Namespace = configuration.GetValue<string>("Logging:RunTimeLogLevel:Namespace");
						options.DefaultLevel = configuration.GetValue<LogLevel>("Logging:RunTimeLogLevel:DefaultLevel");
						options.LogLevelOption = GlobalParameters.Logging;
					})
					.AddLogging(logging =>
					{
						configure.AddConfiguration(configuration.GetSection("Logging"))
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

public static class GlobalParameters
{
	public static LogLevelOption Logging { get; set; }
}

public class ApplicationRootCommand : RootCommand
{
	public ApplicationRootCommand()
	{
		AddGlobalOption(GlobalParameters.Logging);
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