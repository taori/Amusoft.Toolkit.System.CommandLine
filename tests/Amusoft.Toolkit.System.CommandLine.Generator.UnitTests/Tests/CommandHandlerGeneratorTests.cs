using Amusoft.Toolkit.System.CommandLine.Generator.UnitTests.Toolkit;
using Amusoft.Toolkit.System.CommandLine.Generators.CommandHandler;
using Xunit.Abstractions;

namespace Amusoft.Toolkit.System.CommandLine.Generator.UnitTests.Tests;

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
	public async Task Compare(string testFile)
	{
		var testContent = GetProjectFileContent(testFile);

		var results = GetSourceGeneratorResults<SubclassCommandHandlerGenerator>(testContent);

		await Verifier.Verify(results.runResult)
			.UseParameters(testFile);
	}
}