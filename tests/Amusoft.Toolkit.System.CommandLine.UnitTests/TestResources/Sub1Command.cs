using System.CommandLine;
using Amusoft.Toolkit.System.CommandLine.Attributes;
using Microsoft.Extensions.Logging;

namespace Amusoft.Toolkit.System.CommandLine.UnitTests.TestResources;


internal partial class Sub1Command : Command
{
	public Option<string> TestProperty { get; set; } = new Option<string>("--test", "testparameter");

	public Sub1Command() : base("sub1", "sub1 description")
	{
		AddOption(TestProperty);
		// BindHandler();
	}

	[GenerateCommandHandler]
	internal partial class HandlerCommand
	{
		private readonly ILogger<HandlerCommand> _logger;

		public HandlerCommand(ILogger<HandlerCommand> logger)
		{
			_logger = logger;
		}

		// public override Task ExecuteAsync(InvocationContext context)
		// {
		// 	_logger.LogError("test");
		// 	return base.ExecuteAsync(context);
		// }
	}
}