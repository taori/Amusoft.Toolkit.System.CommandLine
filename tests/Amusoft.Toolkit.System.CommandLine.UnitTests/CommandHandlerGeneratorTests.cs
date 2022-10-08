using System.Threading.Tasks;
using Amusoft.Toolkit.System.CommandLine.Generators.CommandHandler;
using Amusoft.Toolkit.System.CommandLine.UnitTests.Toolkit;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

namespace Amusoft.Toolkit.System.CommandLine.UnitTests;

[UsesVerify]
public class CommandHandlerGeneratorTests : GeneratorTestBase
{
	public CommandHandlerGeneratorTests(ITestOutputHelper outputHelper, GlobalSetupFixture data) : base(outputHelper, data)
	{
	}

	[Theory]
	[InlineData("TestResources|ClassWithAttribute.cs")]
	[InlineData("TestResources|ClassWithoutAttribute.cs")]
	[InlineData("TestResources|ClassTwoArguments.cs")]
	[InlineData("TestResources|ClassWrappedOption.cs")]
	[InlineData("TestResources|ClassWrappedArgument.cs")]
	[InlineData("TestResources|Sub1Command.cs")]
	public async Task CompareGeneration(string testFile)
	{
		var testContent = GetProjectFileContent(testFile);
		
		var results = GetSourceGeneratorResults<CommandHandlerGenerator>(testContent);

		await Verifier.Verify(results.runResult)
			.UseParameters(testFile);
	}
}