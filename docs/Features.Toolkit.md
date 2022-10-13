## Code Generation

- Generates implementation of this.SetHandler and uses the options/arguments of the current class
- Utilizes the parameters of the class to generate an execution handler which you have to implement
- Ensures that your implementation is always in sync with parameters
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

		// this sets up the SetHandler call with the generated code
		BindHandler();
	}
	
	public Task ExecuteAsync(InvocationContext context)
	{
		// your code
	}
}
```