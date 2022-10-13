## Code Generation

- Generates implementation of this.SetHandler and uses the options/arguments of the current class
- Utilizes the parameters of the class to generate an execution handler which you have to implement
```cs
[GenerateExecuteHandler]
public partial class ListModGroupCommand : Command
{
	private readonly ILogger<ListModGroupCommand> _logger;
	private readonly IOptions<ApplicationSettings> _applicationSettings;

	public ListModGroupCommand(ILogger<ListModGroupCommand> logger, IOptions<ApplicationSettings> applicationSettings) : base("modgroups", "Lists all modgroups")
	{
		_logger = logger;
		_applicationSettings = applicationSettings;

		BindHandler();
	}
	
	public Task ExecuteAsync(InvocationContext context)
	{
		var sb = new StringBuilder();
		sb.AppendLine("The following groups exist:");
		foreach (var updateDefinition in _applicationSettings.Value.Updates)
		{
			sb.AppendLine(updateDefinition.Id);
		}

		_logger.LogInformation(sb.ToString());

		return Task.CompletedTask;
	}
}
```