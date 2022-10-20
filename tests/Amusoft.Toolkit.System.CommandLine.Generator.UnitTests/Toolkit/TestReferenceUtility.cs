using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using System.CommandLine;
using System.Reflection;
using Amusoft.Toolkit.System.CommandLine.Attributes;
using Microsoft.CodeAnalysis.CodeFixes;

namespace Amusoft.Toolkit.System.CommandLine.Generator.UnitTests.Toolkit;

public static class TestReferenceUtility
{
	public static void ApplyReferences<TLocalAnalyzer>(CSharpAnalyzerTest<TLocalAnalyzer, XUnitVerifier> configuration) where TLocalAnalyzer : DiagnosticAnalyzer, new()
	{
		configuration.ReferenceAssemblies = ReferenceAssemblies.Net.Net50;
		configuration.TestState.AdditionalReferences.AddRange(
			new[]
			{
				MetadataReference.CreateFromFile(typeof(GenerateExecuteHandlerAttribute).GetTypeInfo().Assembly.Location),
				MetadataReference.CreateFromFile(typeof(Command).GetTypeInfo().Assembly.Location)
			});
	}

	public static void ApplyReferences<TAnalyzer, TCodeFix>(CSharpCodeFixTest<TAnalyzer, TCodeFix, XUnitVerifier> configuration) where TAnalyzer : DiagnosticAnalyzer, new() where TCodeFix : CodeFixProvider, new()
	{
		configuration.ReferenceAssemblies = ReferenceAssemblies.Net.Net50;
		configuration.TestState.AdditionalReferences.AddRange(
			new[]
			{
				MetadataReference.CreateFromFile(typeof(GenerateExecuteHandlerAttribute).GetTypeInfo().Assembly.Location),
				MetadataReference.CreateFromFile(typeof(Command).GetTypeInfo().Assembly.Location)
			});
	}
}