using Amusoft.Toolkit.System.CommandLine.CommandModel;
using Amusoft.Toolkit.System.CommandLine.Generator.UnitTests.TestResources.HierarchyGenerator;

namespace Amusoft.Toolkit.System.CommandLine.Generator.UnitTests.Samples;

public static class GeneratedHierarchy
{
	public static readonly ICommandHierarchy ParentCommand = CommandHierarchyBuilder.FromArray(
		new[,]
		{
			{typeof(MixedChainParent), typeof(MixedChainChild1)},
			{typeof(MixedChainParent), typeof(MixedChainChild2)},
		});
}