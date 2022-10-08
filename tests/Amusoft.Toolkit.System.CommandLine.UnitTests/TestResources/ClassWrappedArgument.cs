using System.CommandLine;
using Amusoft.Toolkit.System.CommandLine.Attributes;

namespace Amusoft.Toolkit.System.CommandLine.UnitTests.TestResources;

public partial class ClassWrappedArgument : Command
{
	public CustomArgumentOne ArgumentOne { get; set; }
	public CustomArgumentTwo ArgumentTwo { get; set; }

	public ClassWrappedArgument() : base("sample", "just a sample description")
	{
		AddArgument(ArgumentOne);
		AddArgument(ArgumentTwo);
	}

	[GenerateCommandHandler]
	public partial class ClassWithHandler
	{
	}

	public class CustomArgumentOne : Argument<int>
	{
		public CustomArgumentOne() : base("one", "one")
		{
		}
	}

	public class CustomArgumentTwo : Argument<string>
	{
		public CustomArgumentTwo() : base("two", "two")
		{
		}
	}
}