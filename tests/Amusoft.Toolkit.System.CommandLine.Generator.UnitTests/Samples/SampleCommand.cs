using System.CommandLine;
using System.CommandLine.Invocation;
using Amusoft.Toolkit.System.CommandLine.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog;

namespace Amusoft.Toolkit.System.CommandLine.Generator.UnitTests.Samples;

public partial class SampleCommand : Command
{
	public Option<string> OptionOne { get; set; }

	public Option<int> OptionTwo { get; set; }

	public Argument<int> ArgumentOne { get; set; }

	public SampleCommand() : base("sample", "just a sample description")
	{
		AddOption(OptionOne);
		AddOption(OptionTwo);
		AddArgument(ArgumentOne);

		BindHandler();
	}

	[GenerateCommandHandler]
	public partial class SampleCommandHandler
	{
		private readonly ILogger _logger;

		public SampleCommandHandler(ILogger logger)
		{
			_logger = logger;
		}
	}
}

public partial class SampleCommand 
{
	private SampleCommandHandler _handler;

	private void BindHandler()
	{
		if (_handler is null)
			return;
		
		this.SetHandler(async (context) =>
		{
			var host = context.BindingContext.GetRequiredService<IHost>();
			_handler = new SampleCommandHandler(host.Services.GetRequiredService<ILogger>());

			var p1 = context.ParseResult.GetValueForOption(OptionOne);
			var p2 = context.ParseResult.GetValueForOption(OptionTwo);
			var p3 = context.ParseResult.GetValueForArgument(ArgumentOne);
			
			await _handler.ExecuteAsync(context, p1, p2, p3);
		});
	}

	public partial class SampleCommandHandler : InvokerBase
	{
	}

	public abstract class InvokerBase
	{
		public virtual Task ExecuteAsync(InvocationContext context, string optionOne, int optionTwo, int argumentOne) => Task.CompletedTask;
	}
}