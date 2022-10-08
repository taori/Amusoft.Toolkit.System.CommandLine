using LocalPackageTest.CLI.Toolkit.Options;
using Microsoft.Extensions.Logging;

namespace LocalPackageTest.CLI.Commands;

public class ApplicationRootCommand : System.CommandLine.RootCommand
{

	public static readonly LogLevelOption LogLevelOption = new(LogLevel.Information);

	public ApplicationRootCommand()
	{
		AddCommand(new Sub1Command());
		AddGlobalOption(LogLevelOption);
	}
}