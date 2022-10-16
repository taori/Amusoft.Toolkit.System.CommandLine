using Amusoft.Toolkit.System.CommandLine.Generator.Hierarchy;
using Amusoft.Toolkit.System.CommandLine.Generator.UnitTests.Toolkit;
using Xunit.Abstractions;

namespace Amusoft.Toolkit.System.CommandLine.Generator.UnitTests.Tests;

[UsesVerify]
public class HierarchyTests : GeneratorTestBase
{
	public HierarchyTests(ITestOutputHelper outputHelper, GlobalSetupFixture data) : base(outputHelper, data)
	{
	}

	[Theory]
	[InlineData("TestResources|HierarchyGenerator|ParentToChildren.cs")]
	[InlineData("TestResources|HierarchyGenerator|MixedChain.cs")]
	[InlineData("TestResources|HierarchyGenerator|ParallelChain.cs")]
	public async Task Compare(string testFile)
	{
		var testContent = GetProjectFileContent(testFile);

		var results = GetIncrementalGeneratorResults<HierarchyGenerator>(testContent);

		await Verifier.Verify(results.runResult)
			.UseParameters(testFile);
	}
}