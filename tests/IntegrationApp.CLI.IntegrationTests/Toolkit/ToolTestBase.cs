using System.Reflection;

namespace IntegrationApp.CLI.IntegrationTests.Toolkit;

[Collection(nameof(NoParallelDefinition))]
public class ToolTestBase
{
	protected static Task<DotnetToolIntegrationSession> GetTestSession()
	{
		var packageFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
		var fullPath = new Uri(new Uri(packageFolder, UriKind.Absolute), new Uri("../../integrationApp", UriKind.Relative))
			.LocalPath;
		return DotnetToolIntegrationSession.CreateAsync(fullPath, "integrationapp", "IntegrationApp.CLI");
	}
}