using System.CommandLine;
using Amusoft.Toolkit.System.CommandLine.Attributes;

namespace Amusoft.Toolkit.System.CommandLine.Generator.UnitTests.TestResources.ExecuteHandlerGenerator;

[GenerateExecuteHandler]
internal partial class MixedCommand : Command
{
	public Option<string> TestOption { get; set; } = new Option<string>("--test", "testparameter");

	public Argument<string> TestArgument { get; set; } = new Argument<string>("--test", "testparameter");

	public MixedCommand() : base("sub1", "sub1 description")
	{
		AddOption(TestOption);
		AddArgument(TestArgument);
	}
}