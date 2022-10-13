using Amusoft.Toolkit.System.CommandLine.Generators.CommandHandler;
using Amusoft.Toolkit.System.CommandLine.Generators.ExecuteHandler;
using Amusoft.Toolkit.System.CommandLine.UnitTests.Toolkit;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

namespace Amusoft.Toolkit.System.CommandLine.UnitTests;

[UsesVerify]
public class ExecuteHandlerGeneratorTests : GeneratorTestBase
{
	public ExecuteHandlerGeneratorTests(ITestOutputHelper outputHelper, GlobalSetupFixture data) : base(outputHelper, data)
	{
	}

	[Theory]
	[InlineData("TestResources|ExecuteHandlerGenerator|MixedCommand.cs")]
	[InlineData("TestResources|ExecuteHandlerGenerator|MixedCommandNoParameter.cs")]
	public async Task CompareGeneration(string testFile)
	{
		var testContent = GetProjectFileContent(testFile);

		var results = GetIncrementalGeneratorResults<ExecuteHandlerGenerator>(testContent);

		await Verifier.Verify(results.runResult)
			.UseParameters(testFile);
	}
}