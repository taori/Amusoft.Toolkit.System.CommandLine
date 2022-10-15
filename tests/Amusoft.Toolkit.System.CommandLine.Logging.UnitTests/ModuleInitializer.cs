using System.Runtime.CompilerServices;

namespace Amusoft.Toolkit.System.CommandLine.Logging.UnitTests;

internal class ModuleInitializer
{
	[ModuleInitializer]
	public static void Initialize()
	{
		VerifierSettings.DerivePathInfo(
			(sourceFile, projectDirectory, type, method) => new(
				directory: Path.Combine(projectDirectory, "Snapshots"),
				typeName: type.Name,
				methodName: method.Name));
	}
}