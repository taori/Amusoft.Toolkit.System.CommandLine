using Amusoft.XUnit.NLog.Extensions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Xunit.Abstractions;

namespace Amusoft.Toolkit.System.CommandLine.Generator.UnitTests.Toolkit;

public class CodeFixTestBase<TAnalyzer, TCodeFix> : LoggedTestBase
	where TAnalyzer : DiagnosticAnalyzer, new()
	where TCodeFix : CodeFixProvider, new()
{
	public CodeFixTestBase(ITestOutputHelper outputHelper) : base(outputHelper)
	{
	}

	protected CSharpCodeFixTest<TAnalyzer, TCodeFix, XUnitVerifier> CreateTest()
	{
		var test = new CSharpCodeFixTest<TAnalyzer, TCodeFix, XUnitVerifier>();
		TestReferenceUtility.ApplyReferences(test);
		return test;
	}

	protected DiagnosticResult DiagnosticById(string diagnosticId) => CSharpCodeFixVerifier<TAnalyzer, TCodeFix, XUnitVerifier>.Diagnostic(diagnosticId);

	protected CSharpCodeFixTest<TAnalyzer, TCodeFix, XUnitVerifier> CreateTest(string input, DiagnosticResult[] diagnosticResults, string expectedOutput)
	{
		var test = CreateTest();
		test.TestCode = input;
		test.FixedCode = expectedOutput;
		test.TestState.ExpectedDiagnostics.AddRange(diagnosticResults);

		return test;
	}
}