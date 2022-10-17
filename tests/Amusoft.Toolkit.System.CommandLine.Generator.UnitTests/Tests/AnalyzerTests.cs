using System.Collections.Immutable;
using System.CommandLine;
using System.Reflection;
using Amusoft.Toolkit.System.CommandLine.Generator.Analyzers;
using Amusoft.Toolkit.System.CommandLine.Generator.UnitTests.Toolkit;
using Amusoft.Toolkit.System.CommandLine.Generators.ExecuteHandler;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Xunit.Abstractions;
using BindHandlerAnalyzerVerifier = 
	Amusoft.Toolkit.System.CommandLine.Generator.UnitTests.Tests.ExtendedAnalyzerVerifier<Amusoft.Toolkit.System.CommandLine.Generator.Analyzers.CommandCallsBindHandlerAnalyzer>;
// using BindHandlerAnalyzerVerifier =
// 	Microsoft.CodeAnalysis.CSharp.Testing.XUnit.AnalyzerVerifier<Amusoft.Toolkit.System.CommandLine.Generator.Analyzers.CommandCallsBindHandlerAnalyzer>;

namespace Amusoft.Toolkit.System.CommandLine.Generator.UnitTests.Tests;

public class AnalyzerTests :GeneratorTestBase
{
	public AnalyzerTests(ITestOutputHelper outputHelper, GlobalSetupFixture data) : base(outputHelper, data)
	{
	}

	[Fact]
	async Task VerifyNoDiagnosticIfBindCalled()
	{
		var input = """
		using System.CommandLine;
		namespace Amusoft.Toolkit.System.CommandLine.Generator.UnitTests.Tests;

		public partial class TestCommand : Command
		{
			public TestCommand() : base("")
			{
				BindHandler();
			}

			private void BindHandler() { }
		}
		""";
		
		await BindHandlerAnalyzerVerifier.VerifyAnalyzerAsync(input, Configure);
	}

	[Fact]
	async Task VerifyDiagnosticIfBindNotCalled()
	{
		var input = """
		using System.CommandLine;
		namespace Amusoft.Toolkit.System.CommandLine.Generator.UnitTests.Tests;

		public partial class TestCommand : Command
		{
			public TestCommand() : base("")
			{
			}

			private void BindHandler() { }
		}
		""";
		
		var diagnostic = BindHandlerAnalyzerVerifier.Diagnostic().WithSpan(4, 1, 11, 2).WithArguments("TestCommand");
		await BindHandlerAnalyzerVerifier.VerifyAnalyzerAsync(input, Configure, diagnostic);
	}

	private void Configure(CSharpAnalyzerTest<CommandCallsBindHandlerAnalyzer, XUnitVerifier> configuration)
	{
		configuration.ReferenceAssemblies = ReferenceAssemblies.Net.Net50;

		configuration.TestState.AdditionalReferences.AddRange(
			new[]
			{
				MetadataReference.CreateFromFile(typeof(Command).GetTypeInfo().Assembly.Location)
			});
	}
}

public class ExtendedAnalyzerVerifier<TAnalyzer> : ExtendedAnalyzerVerifier<TAnalyzer, CSharpAnalyzerTest<TAnalyzer, XUnitVerifier>, XUnitVerifier>
	where TAnalyzer : DiagnosticAnalyzer, new(){}

public class ExtendedAnalyzerVerifier<TAnalyzer, TTest, TVerifier>
	where TAnalyzer : DiagnosticAnalyzer, new()
	where TTest : AnalyzerTest<TVerifier>, new()
	where TVerifier : IVerifier, new()
{
	public static DiagnosticResult Diagnostic()
	{
		var analyzer = new TAnalyzer();
		try
		{
			return Diagnostic(analyzer.SupportedDiagnostics.Single());
		}
		catch (InvalidOperationException ex)
		{
			throw new InvalidOperationException(
				$"'{nameof(Diagnostic)}()' can only be used when the analyzer has a single supported diagnostic. Use the '{nameof(Diagnostic)}(DiagnosticDescriptor)' overload to specify the descriptor from which to create the expected result.",
				ex);
		}
	}
	
	public static DiagnosticResult Diagnostic(string diagnosticId)
	{
		var analyzer = new TAnalyzer();
		try
		{
			return Diagnostic(analyzer.SupportedDiagnostics.Single(i => i.Id == diagnosticId));
		}
		catch (InvalidOperationException ex)
		{
			throw new InvalidOperationException(
				$"'{nameof(Diagnostic)}(string)' can only be used when the analyzer has a single supported diagnostic with the specified ID. Use the '{nameof(Diagnostic)}(DiagnosticDescriptor)' overload to specify the descriptor from which to create the expected result.",
				ex);
		}
	}
	
	public static DiagnosticResult Diagnostic(DiagnosticDescriptor descriptor) => new DiagnosticResult(descriptor);

	public static Task VerifyAnalyzerAsync(string source, Action<TTest>? configure = default, params DiagnosticResult[] expected)
	{
		var test = new TTest
		{
			TestCode = source,
		};

		test.ExpectedDiagnostics.AddRange(expected);
		configure?.Invoke(test);
		
		return test.RunAsync(CancellationToken.None);
	}
}