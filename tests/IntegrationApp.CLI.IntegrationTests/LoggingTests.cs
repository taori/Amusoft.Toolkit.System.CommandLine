using IntegrationApp.CLI.IntegrationTests.Toolkit;
using Microsoft.VisualBasic;
using Xunit.Abstractions;

namespace IntegrationApp.CLI.IntegrationTests
{
	[UsesVerify]
	public class LoggingTests : ToolTestBase
	{
		private readonly ITestOutputHelper _outputHelper;

		public LoggingTests(ITestOutputHelper outputHelper)
		{
			_outputHelper = outputHelper;
		}

		[Theory]
		[InlineData("test")]
		[InlineData("test2")]
		[InlineData("-h")]
		[InlineData("test --logLevel Warning")]
		[InlineData("test2 --logLevel Warning")]
		public async Task VerifyLogSwitch(string command)
		{
			await using var session = await GetTestSession();
			await session.ExecuteAsync(command);

			// _outputHelper.WriteLine(session.Output.ToString());
			// _outputHelper.WriteLine(session.Error.ToString());

			await Verify(session.Output).UseParameters(command);
			await Verify(session.Error).UseParameters(command);
		}
	}
}