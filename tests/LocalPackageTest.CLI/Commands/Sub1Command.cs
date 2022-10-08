using System.CommandLine;
using System.CommandLine.Invocation;
using Amusoft.Generators.System.CommandLine.Attributes;
using Microsoft.Extensions.Logging;

namespace LocalPackageTest.CLI.Commands;

internal partial class Sub1Command : Command
{
	public Option<string> TestProperty { get; set; } = new Option<string>("--test", "testparameter");

	public Option<int> Number { get; set; } = new("--number", "Some number");

	public Sub1Command() : base("sub1", "sub1 description")
	{
		AddOption(TestProperty);
		AddOption(Number);
		BindHandler();
	}

	[GenerateCommandHandler]
	internal partial class HandlerCommand
	{
		private readonly ILogger<HandlerCommand> _logger;

		public HandlerCommand(ILogger<HandlerCommand> logger)
		{
			_logger = logger;
		}

		public override Task ExecuteAsync(InvocationContext context, string testProperty, int number)
		{
			_logger.LogError("Printing parameter {test} {Number}", testProperty, number);
			return base.ExecuteAsync(context, testProperty, number);
		}
	}
}