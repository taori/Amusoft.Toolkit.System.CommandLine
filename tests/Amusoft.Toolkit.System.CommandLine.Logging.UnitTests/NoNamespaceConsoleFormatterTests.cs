using Amusoft.Toolkit.System.CommandLine.Logging.Formatters;
using Amusoft.Toolkit.System.CommandLine.Logging.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;

namespace Amusoft.Toolkit.System.CommandLine.Logging.UnitTests
{
	[UsesVerify]
	public class NoNamespaceConsoleFormatterTests
	{
		[Fact]
		public void Test1()
		{
			// CreateDefaultLogMessage
			var optionsMock = new Mock<IOptionsMonitor<NoNamespaceConsoleFormatterOptions>>();
			optionsMock.SetupGet(d => d.CurrentValue).Returns(new NoNamespaceConsoleFormatterOptions());
			var formatter = new NoNamespaceConsoleFormatter(optionsMock.Object);
			var input = """
			line1
			line2
			line3
			""";

			var stringWriter = RunCreateDefaultLogMessage(formatter, input);
			Verify(stringWriter);
		}

		private static string RunCreateDefaultLogMessage(NoNamespaceConsoleFormatter formatter, string message)
		{
			var stringWriter = new StringWriter();
			formatter.CreateDefaultLogMessage(stringWriter, new LogEntry<object>(LogLevel.Information, "SomeCategory", new EventId(0), null, null, (o, exception) => exception?.ToString()), message, new LoggerExternalScopeProvider());
			return stringWriter.ToString();
		}
	}
}