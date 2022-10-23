using Amusoft.Toolkit.System.CommandLine.Generator.ExecuteHandler;
using Amusoft.Toolkit.System.CommandLine.Generator.UnitTests.Toolkit;
using Xunit.Abstractions;

namespace Amusoft.Toolkit.System.CommandLine.Generator.UnitTests.Tests.Gen;

[UsesVerify]
public class GenerateExecuteTests : GeneratorTestBase
{
    public GenerateExecuteTests(ITestOutputHelper outputHelper, GlobalSetupFixture data) : base(outputHelper, data)
    {
    }

    [Theory]
    [InlineData("TestResources|ExecuteHandlerGenerator|MixedCommand.cs")]
    [InlineData("TestResources|ExecuteHandlerGenerator|MixedCommandNoParameter.cs")]
    public async Task Compare(string testFile)
    {
        var testContent = GetProjectFileContent(testFile);

        var results = GetIncrementalGeneratorResults<ExecuteHandlerGenerator>(testContent);

        await Verify(results.runResult)
            .UseParameters(testFile);
    }
}