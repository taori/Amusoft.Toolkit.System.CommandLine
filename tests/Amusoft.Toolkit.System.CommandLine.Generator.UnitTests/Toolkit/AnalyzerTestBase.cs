using System.CommandLine;
using System.Reflection;
using Amusoft.Toolkit.System.CommandLine.Attributes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Xunit.Abstractions;

namespace Amusoft.Toolkit.System.CommandLine.Generator.UnitTests.Toolkit;

public class AnalyzerTestBase<TAnalyzer> : GeneratorTestBase
	where TAnalyzer : DiagnosticAnalyzer, new()
{
	protected void Configure(CSharpAnalyzerTest<TAnalyzer, XUnitVerifier> configuration)
	{
		configuration.ReferenceAssemblies = ReferenceAssemblies.Net.Net50;
		configuration.TestState.AdditionalReferences.AddRange(
			new[]
			{
				MetadataReference.CreateFromFile(typeof(GenerateExecuteHandlerAttribute).GetTypeInfo().Assembly.Location),
				MetadataReference.CreateFromFile(typeof(Command).GetTypeInfo().Assembly.Location)
			});
	}

	public AnalyzerTestBase(ITestOutputHelper outputHelper, GlobalSetupFixture data) : base(outputHelper, data)
	{
	}
}