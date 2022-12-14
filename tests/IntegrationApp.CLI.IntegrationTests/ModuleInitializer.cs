using System.Runtime.CompilerServices;

namespace IntegrationApp.CLI.IntegrationTests;

internal class ModuleInitializer
{
	[ModuleInitializer]
	public static void Initialize()
	{
		Verifier.DerivePathInfo(
			(sourceFile, projectDirectory, type, method) => new(
				directory: Path.Combine(projectDirectory, "Snapshots"),
				typeName: type.Name,
				methodName: method.Name));
	}
}