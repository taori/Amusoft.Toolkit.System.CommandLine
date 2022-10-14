using System.CommandLine;
using Amusoft.Toolkit.System.CommandLine.Attributes;

namespace Amusoft.Toolkit.System.CommandLine.Generator.UnitTests.TestResources;

public partial class ClassWrappedOption : Command
{
	public CustomOptionOne OptionOne { get; set; }
	public CustomOptionTwo OptionTwo { get; set; }

	public ClassWrappedOption() : base("sample", "just a sample description")
	{
		AddOption(OptionOne);
		AddOption(OptionTwo);
	}

	[GenerateCommandHandler]
	public partial class ClassWithHandler
	{
	}

	public class CustomOptionOne : Option<int>
	{
		public CustomOptionOne() : base("one", "one")
		{
		}
	}

	public class CustomOptionTwo : Option<string>
	{
		public CustomOptionTwo() : base("two", "two")
		{
		}
	}
}