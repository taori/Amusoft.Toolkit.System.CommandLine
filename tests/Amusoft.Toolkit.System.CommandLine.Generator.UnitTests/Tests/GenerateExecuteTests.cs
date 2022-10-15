using Amusoft.Toolkit.System.CommandLine.Generator.UnitTests.Toolkit;
using Amusoft.Toolkit.System.CommandLine.Generators.ExecuteHandler;
using Xunit.Abstractions;

namespace Amusoft.Toolkit.System.CommandLine.Generator.UnitTests.Tests;

[UsesVerify]
public class GenerateExecuteTests : GeneratorTestBase
{
	public GenerateExecuteTests (ITestOutputHelper outputHelper, GlobalSetupFixture data) : base(outputHelper, data)
	{
	}

	[Theory]
	[InlineData("TestResources|ExecuteHandlerGenerator|MixedCommand.cs")]
	[InlineData("TestResources|ExecuteHandlerGenerator|MixedCommandNoParameter.cs")]
	public async Task Compare(string testFile)
	{
		var testContent = GetProjectFileContent(testFile);

		var results = GetIncrementalGeneratorResults<ExecuteHandlerGenerator>(testContent);

		await Verifier.Verify(results.runResult)
			.UseParameters(testFile);
	}
}