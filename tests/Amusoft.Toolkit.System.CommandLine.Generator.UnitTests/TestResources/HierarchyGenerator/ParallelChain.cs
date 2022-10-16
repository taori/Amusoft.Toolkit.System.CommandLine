using System.CommandLine;
using Amusoft.Toolkit.System.CommandLine.Attributes;

namespace Amusoft.Toolkit.System.CommandLine.Generator.UnitTests.TestResources.HierarchyGenerator;

[HasChildCommand(typeof(ParallelChainChild1))]
public class ParallelChainParent : RootCommand
{
}

public class ParallelChainChild1 : Command
{
	public ParallelChainChild1() : base(string.Empty, string.Empty)
	{
	}
}

[HasParentCommand(typeof(ParallelChainChild1))]
public class ParallelChainChild2 : Command
{
	public ParallelChainChild2() : base(string.Empty, string.Empty)
	{
	}
}


[HasChildCommand(typeof(Parallel2ChainChild1))]
public class Parallel2ChainParent : RootCommand
{
}

public class Parallel2ChainChild1 : Command
{
	public Parallel2ChainChild1() : base(string.Empty, string.Empty)
	{
	}
}

[HasParentCommand(typeof(Parallel2ChainChild1))]
public class Parallel2ChainChild2 : Command
{
	public Parallel2ChainChild2() : base(string.Empty, string.Empty)
	{
	}
}