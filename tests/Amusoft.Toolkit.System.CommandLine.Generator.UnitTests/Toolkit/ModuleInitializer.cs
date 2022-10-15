using System.Runtime.CompilerServices;

namespace Amusoft.Toolkit.System.CommandLine.Generator.UnitTests.Toolkit;

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

		VerifySourceGenerators.Enable();
	}
}