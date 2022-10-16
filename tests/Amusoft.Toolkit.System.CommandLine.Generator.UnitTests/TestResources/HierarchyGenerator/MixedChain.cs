using System.CommandLine;
using Amusoft.Toolkit.System.CommandLine.Attributes;

namespace Amusoft.Toolkit.System.CommandLine.Generator.UnitTests.TestResources.HierarchyGenerator;

[HasChildCommand(typeof(MixedChainChild1))]
public class MixedChainParent : RootCommand
{
}

public class MixedChainChild1 : Command
{
	public MixedChainChild1() : base(string.Empty, string.Empty)
	{
	}
}

[HasParentCommand(typeof(MixedChainChild1))]
public class MixedChainChild2 : Command
{
	public MixedChainChild2() : base(string.Empty, string.Empty)
	{
	}
}