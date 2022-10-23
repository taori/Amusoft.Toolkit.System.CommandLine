using IntegrationApp.CLI.IntegrationTests.Toolkit;
using Microsoft.VisualBasic;
using Xunit.Abstractions;

namespace IntegrationApp.CLI.IntegrationTests
{
	public class TestSession : ToolTestBase, IAsyncLifetime
	{
		public DotnetToolIntegrationSession Session { get; private set; }
		
		public async Task InitializeAsync()
		{
			Session = await GetTestSession().ConfigureAwait(false);

			Console.WriteLine(Session.Output.ToString());
			Console.WriteLine(Session.Error.ToString());
		}

		public async Task DisposeAsync()
		{
			await Session.DisposeAsync();
		}
	}

	[UsesVerify]
	public class LoggingTests : ToolTestBase, IClassFixture<TestSession>
	{
		private readonly ITestOutputHelper _outputHelper;
		private readonly TestSession _session;

		public LoggingTests(ITestOutputHelper outputHelper, TestSession session)
		{
			_outputHelper = outputHelper;
			_session = session;
		}

		[Theory]
		[InlineData("test")]
		[InlineData("test2")]
		[InlineData("-h")]
		[InlineData("test --logLevel Warning")]
		[InlineData("test2 --logLevel Warning")]
		public async Task VerifyLogSwitch(string command)
		{
			var session = _session.Session;
			session.Output.Clear();
			session.Error.Clear();

			await session.ExecuteAsync(command);
			// await using var session = await GetTestSession();

			await Verify(new
			{
				session.Output,
				session.Error
			}).UseParameters(command);
		}
	}
}