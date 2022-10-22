using System.Text;
using CliWrap;
using CliWrap.Buffered;

namespace IntegrationApp.CLI.IntegrationTests.Toolkit;

public class DotnetToolIntegrationSession : IAsyncDisposable
{
	private readonly string _packageFolder;
	private readonly string _toolName;
	private readonly string _packageId;

	public readonly StringBuilder Output = new StringBuilder();

	public readonly StringBuilder Error = new StringBuilder();

	public static async Task<DotnetToolIntegrationSession> CreateAsync(string packageFolder, string toolName, string packageId)
	{
		var session = new DotnetToolIntegrationSession(packageFolder, toolName, packageId);
		await session.InstallAsync();
		return session;
	}

	private async Task InstallAsync()
	{
		await RunAppendAsync("dotnet", $"tool install --global --add-source {_packageFolder} {_packageId} --prerelease");
	}

	private async Task RunAppendAsync(string targetFilePath, string arguments)
	{
		var result = await Cli.Wrap(targetFilePath)
			.WithArguments(arguments)
			.ExecuteBufferedAsync();

		Output.Append(result.StandardOutput);
		Error.Append(result.StandardError);
	}

	private async Task UninstallAsync()
	{
		await RunAppendAsync("dotnet", $"dotnet tool uninstall {_packageId} --global");
	}

	private DotnetToolIntegrationSession(string packageFolder, string toolName, string packageId)
	{
		_packageFolder = packageFolder;
		_toolName = toolName;
		_packageId = packageId;
	}

	public async ValueTask DisposeAsync()
	{
		await UninstallAsync();
	}

	public async Task ExecuteAsync(string command)
	{
		await RunAppendAsync(_toolName, command);
	}
}