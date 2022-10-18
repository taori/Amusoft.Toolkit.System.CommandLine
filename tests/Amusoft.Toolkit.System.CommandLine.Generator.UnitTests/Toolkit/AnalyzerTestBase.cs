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
	public AnalyzerTestBase(ITestOutputHelper outputHelper, GlobalSetupFixture data) : base(outputHelper, data)
	{
	}

	protected static void ConfigureDefaultTest<TLocalAnalyzer>(CSharpAnalyzerTest<TLocalAnalyzer, XUnitVerifier> configuration)
		where TLocalAnalyzer : DiagnosticAnalyzer, new()
	{
		configuration.ReferenceAssemblies = ReferenceAssemblies.Net.Net50;
		configuration.TestState.AdditionalReferences.AddRange(
			new[]
			{
				MetadataReference.CreateFromFile(typeof(GenerateExecuteHandlerAttribute).GetTypeInfo().Assembly.Location),
				MetadataReference.CreateFromFile(typeof(Command).GetTypeInfo().Assembly.Location)
			});
	}

	protected static DiagnosticResult Diagnostic(string? diagnosticId, bool fullId = false)
	{
		if (diagnosticId == null)
		{
			return ExtendedAnalyzerVerifier<TAnalyzer>.Diagnostic(diagnosticId, fullId);
		}
		else
		{
			return ExtendedAnalyzerVerifier<TAnalyzer>.Diagnostic();
		}
	}

	protected async Task RunTestAsync(string input, params DiagnosticResult[] diagnostics)
	{
		await RunTestAsync(input, test => { }, diagnostics);
	}

	protected async Task RunTestAsync(string input, Action<CSharpAnalyzerTest<TAnalyzer, XUnitVerifier>> configuration, params DiagnosticResult[] diagnostics)
	{
		await ExtendedAnalyzerVerifier<TAnalyzer>.VerifyAnalyzerAsync(input, test =>
		{
			ConfigureDefaultTest(test);
			configuration(test);
		}, diagnostics);
	}

	protected async Task RunTestAsync<TLocalAnalyzer>(string input, params DiagnosticResult[] diagnostics)
		where TLocalAnalyzer : DiagnosticAnalyzer, new()
	{
		await RunTestAsync<TLocalAnalyzer>(input, diagnostics);
	}

	protected async Task RunTestAsync<TLocalAnalyzer>(string input, Action<CSharpAnalyzerTest<TLocalAnalyzer, XUnitVerifier>> configuration, params DiagnosticResult[] diagnostics)
		where TLocalAnalyzer : DiagnosticAnalyzer, new()
	{
		await RunTestAsync(input, configuration, diagnostics);
	}
}