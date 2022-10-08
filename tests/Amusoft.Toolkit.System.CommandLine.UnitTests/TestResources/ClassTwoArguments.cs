using System.CommandLine;
using Amusoft.Toolkit.System.CommandLine.Attributes;

namespace Amusoft.Toolkit.System.CommandLine.UnitTests.TestResources;

public partial class ClassTwoArguments : Command
{
	public Argument<int> ArgumentOne { get; set; }
	public Argument<int> ArgumentTwo { get; set; }

	public ClassTwoArguments() : base("sample", "just a sample description")
	{
		AddArgument(ArgumentOne);
		AddArgument(ArgumentTwo);
	}

	[GenerateCommandHandler]
	public partial class ClassWithHandler
	{
		private readonly string _logger;

		public ClassWithHandler(string logger, int someOtherService)
		{
			_logger = logger;
		}
	}
}