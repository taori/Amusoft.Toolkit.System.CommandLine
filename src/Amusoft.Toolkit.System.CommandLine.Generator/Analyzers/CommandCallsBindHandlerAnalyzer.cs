using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using NotImplementedException = System.NotImplementedException;

namespace Amusoft.Toolkit.System.CommandLine.Generator.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
internal class CommandCallsBindHandlerAnalyzer : DiagnosticAnalyzer
{
	private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
		"ATSCG001", 
		"Method call missing", 
		"BindHandler must be called in Constructor of {0}",
		"Amusoft.Toolkit.System.CommandLine.Generator Diagnostics", DiagnosticSeverity.Error, isEnabledByDefault: true,
		description: "Some Description");

	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
	{
		get { return ImmutableArray.Create(Rule); }
	}

	public override void Initialize(AnalysisContext context)
	{
		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
		context.RegisterSyntaxNodeAction(AnalyzeClassNode, SyntaxKind.ClassDeclaration);
		// context.RegisterSymbolAction(AnalyzeNamedType, SymbolKind.NamedType);
		context.EnableConcurrentExecution();
	}

	private void AnalyzeClassNode(SyntaxNodeAnalysisContext context)
	{
		if (context.Node is ClassDeclarationSyntax classDeclaration)
		{
			if (classDeclaration is {BaseList.Types : { } types} && types is {Count: > 0})
				context.ReportDiagnostic(Diagnostic.Create(Rule, context.Node.GetLocation(), classDeclaration.Identifier.Text));
		}
	}
}