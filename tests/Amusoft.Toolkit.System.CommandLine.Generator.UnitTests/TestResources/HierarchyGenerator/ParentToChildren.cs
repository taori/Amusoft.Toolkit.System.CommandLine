using System.CommandLine;
using Amusoft.Toolkit.System.CommandLine.Attributes;

namespace Amusoft.Toolkit.System.CommandLine.Generator.UnitTests.TestResources.HierarchyGenerator;

[HasChildCommand(typeof(Child1Command))]
[HasChildCommand(typeof(Child2Command))]
public class ParentCommand : RootCommand
{
}

public class Child1Command : Command
{
	public Child1Command() : base(string.Empty, string.Empty)
	{
	}
}

public class Child2Command : Command
{
	public Child2Command () : base(string.Empty, string.Empty)
	{
	}
}