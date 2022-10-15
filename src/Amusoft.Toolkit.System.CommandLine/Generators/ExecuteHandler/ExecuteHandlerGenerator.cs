﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Security;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Amusoft.Toolkit.System.CommandLine.Attributes;
using Amusoft.Toolkit.System.CommandLine.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Amusoft.Toolkit.System.CommandLine.Generators.ExecuteHandler;


[Generator]
internal class ExecuteHandlerGenerator : IIncrementalGenerator
{
	private void AppendGeneratorCode(GeneratorExecutionContext context, SemanticModel semanticModel, ClassDeclarationSyntax outerClass)
	{
		if (outerClass == null) throw new ArgumentNullException(nameof(outerClass));

		var outerClassSymbol = semanticModel.GetDeclaredSymbol(outerClass);
		if (outerClassSymbol is null)
			return;

		var targetNamespace = outerClassSymbol.ContainingNamespace.ToDisplayString();
		var sb = new StringBuilder();

		if (!outerClass.SyntaxTree.TryGetRoot(out var root))
			return;

		var fileUsings = root.DescendantNodes().OfType<UsingDirectiveSyntax>()
			.Select(d => d.ToString());
		var allUsings = new HashSet<string>(new[]
			{
				"using System.CommandLine;",
				"using System.CommandLine.Invocation;",
				"using System.Threading.Tasks;",
				"using Amusoft.Toolkit.System.CommandLine.Attributes;",
				"using Microsoft.Extensions.DependencyInjection;",
				"using Microsoft.Extensions.Hosting;",

			}
		);
		allUsings.UnionWith(fileUsings);
		var usingList = string.Join(Environment.NewLine, allUsings);

		sb.Append(
		$$"""
		// <auto-generated/>
		{{usingList}}

		namespace {{targetNamespace}};	
		
		{{outerClass.Modifiers}} class {{outerClass.Identifier}} : I{{outerClass.Identifier.Text}}
		{
			private void BindHandler()
			{
				this.SetHandler(async (context) =>
				{	
		""");

		var commandParameterDeclarations = GetCommandParameterDeclarations(semanticModel, outerClass, CancellationToken.None).ToArray();
		foreach (var parameterDeclaration in commandParameterDeclarations)
		{
			sb.Append(
		$$"""
					var {{parameterDeclaration.InvokeName}} = context.ParseResult.{{parameterDeclaration.ParseResultCallName}}({{parameterDeclaration.OptionName}});
	
		""");

		}

		if (commandParameterDeclarations.Length == 0)
		{
			sb.Append(
		$$"""
					await ExecuteAsync(context);
	
		""");
		}
		else
		{
			sb.Append(
		$$"""
					await ExecuteAsync(context, {{string.Join(", ", commandParameterDeclarations.Select(d => d.InvokeName))}});
	
		""");
		}

		sb.Append(
		$$"""
				});
			}
		}
	
		{{outerClass.Modifiers}} interface I{{outerClass.Identifier.Text}}
		{
		}
		""");

		context.AddSource($"{outerClass.Identifier.Text}.g.cs", sb.ToString());
	}

	private record ParameterDeclaration(string InvokeName, string OptionName, string ParseResultCallName, string UnwrappedOptionType);

	private static IEnumerable<ParameterDeclaration> GetCommandParameterDeclarations(SemanticModel semanticModel, ClassDeclarationSyntax outerClass, CancellationToken cancellationToken)
	{
		var index = 1;
		var optionType = semanticModel.Compilation.GetTypeByMetadataName("System.CommandLine.Option`1");
		var argumentType = semanticModel.Compilation.GetTypeByMetadataName("System.CommandLine.Argument`1");

		foreach (var memberDeclarationSyntax in outerClass.Members)
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (memberDeclarationSyntax is PropertyDeclarationSyntax propertyDeclaration)
			{
				if (semanticModel.GetSymbolInfo(propertyDeclaration.Type) is { Symbol: { } propertyTypeSymbol })
				{
					if (IsPropertyOfQueryType(propertyTypeSymbol, optionType, out var unwrappedType))
						yield return new ParameterDeclaration($"p{index++}", propertyDeclaration.Identifier.Text, "GetValueForOption", unwrappedType);
					if (IsPropertyOfQueryType(propertyTypeSymbol, argumentType, out unwrappedType))
						yield return new ParameterDeclaration($"p{index++}", propertyDeclaration.Identifier.Text, "GetValueForArgument", unwrappedType);
				}
			}
		}
	}

	private static bool IsPropertyOfQueryType(ISymbol propertyTypeSymbol, INamedTypeSymbol? propertyQueryType, [NotNullWhen(true)] out string? unwrappedQueryType)
	{
		unwrappedQueryType = default;
		if (propertyTypeSymbol is INamedTypeSymbol namedType)
		{
			var match = propertyTypeSymbol.OriginalDefinition.Equals(propertyQueryType, SymbolEqualityComparer.Default);
			if (match)
			{
				if (namedType.TypeArguments.Length > 0)
				{
					unwrappedQueryType = namedType.TypeArguments[0].ToDisplayString();
				}
			}
			else
			{
				if (namedType.BaseType is not null && IsPropertyOfQueryType(namedType.BaseType, propertyQueryType, out unwrappedQueryType))
					return true;
			}

			return match;
		}

		return false;
	}

	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		var classesWithTargetGenerator = context.SyntaxProvider.ForAttributeWithMetadataName(typeof(GenerateExecuteHandlerAttribute).FullName!,
			static (c, _) => c is ClassDeclarationSyntax,
			static (context, token) => GetClassGenerationResult(context, token));

		context.RegisterSourceOutput(classesWithTargetGenerator, GenerateSources);
	}

	private static ClassGenerationResult? GetClassGenerationResult(GeneratorAttributeSyntaxContext context, CancellationToken cancellationToken)
	{
		if (context.TargetNode is ClassDeclarationSyntax { } classDeclaration && context.SemanticModel.GetDeclaredSymbol(classDeclaration) is {} classSymbol)
		{
			var parameters = GetCommandParameterDeclarations(context.SemanticModel, classDeclaration, cancellationToken).ToArray();
			return new ClassGenerationResult(classDeclaration, parameters, classSymbol);
		}

		return default;
	}

	private record struct ClassGenerationResult(ClassDeclarationSyntax Class, ParameterDeclaration[] Properties, ISymbol ClassSymbol);

	private void GenerateSources(SourceProductionContext context, ClassGenerationResult? classGeneration)
	{
		if (classGeneration == null)
			return;

		var generation = classGeneration.Value;

		var sb = new StringBuilder();
		AppendGeneratedCode(sb, generation);
		context.AddSource($"{generation.Class.Identifier.Text}.g.cs", sb.ToString());
	}

	private void AppendGeneratedCode(StringBuilder sb, ClassGenerationResult classGeneration)
	{
		// AppendGeneratorCode(null, context.);

		var targetNamespace = classGeneration.ClassSymbol.ContainingNamespace.ToDisplayString();

		if (!classGeneration.Class.SyntaxTree.TryGetRoot(out var root))
			return;

		var fileUsings = root.DescendantNodes().OfType<UsingDirectiveSyntax>()
			.Select(d => d.ToString());
		var allUsings = new HashSet<string>(new[]
			{
				"using System.CommandLine;",
				"using System.CommandLine.Invocation;",
				"using System.Threading.Tasks;",
				"using Amusoft.Toolkit.System.CommandLine.Attributes;",
				"using Microsoft.Extensions.DependencyInjection;",
				"using Microsoft.Extensions.Hosting;",

			}
		);
		allUsings.UnionWith(fileUsings);
		var usingList = string.Join(Environment.NewLine, allUsings);

		sb.Append($$"""
		// <auto-generated/>

		{{usingList}}
		
		namespace {{targetNamespace}};	

		{{classGeneration.Class.Modifiers}} class {{classGeneration.Class.Identifier.Text}} : {{classGeneration.Class.Identifier.Text}}.ICommandInvoker
		{		
		{{BindHandler(classGeneration)}}

		{{GetInterface(classGeneration)}}
		}
		""");

		string BindHandler(ClassGenerationResult p)
		{
			var sb = new StringBuilder();

			var argumentDeclaration = string.Join("", p.Properties.Select(d => $"\t\t\tvar {d.InvokeName} = context.ParseResult.{d.ParseResultCallName}({d.OptionName});{Environment.NewLine}"));
			var parameterList = string.Join("", p.Properties.Select(d => $", {d.InvokeName}"));
			sb.Append($$"""		
				private void BindHandler()
				{
					this.SetHandler(async (context) =>
					{
			{{argumentDeclaration}}			
						await (({{classGeneration.Class.Identifier.Text}}.ICommandInvoker)this).ExecuteAsync(context{{parameterList}});
					});
				}
			""");

			return sb.ToString();
		}

		string GetInterface(ClassGenerationResult p)
		{
			var sb = new StringBuilder();

			var parameterList = string.Join("", p.Properties.Select(d => $", {d.UnwrappedOptionType} {d.OptionName.ToCamelCase()}"));
			sb.Append($$"""			
				private interface ICommandInvoker
				{
					public abstract Task ExecuteAsync(InvocationContext context{{parameterList}});
				}
			""");

			return sb.ToString();
		}

		// public class SomeCommand : Command, SomeCommand.IClassName
		// {
		//
		// 	private void BindHandler()
		// 	{
		// 		this.SetHandler(async (context) =>
		// 		{
		// 			var p1 = context.ParseResult.GetValueForOption(TestOption);
		// 			var p2 = context.ParseResult.GetValueForArgument(TestArgument);
		//
		// 			// await ExecuteAsync(context, p1, p2);
		// 			await Task.CompletedTask;
		// 		});
		// 	}
		//
		// 	private interface IClassName
		// 	{
		// 		public Task ExecuteAsync(InvocationContext context, string optionOne, string optionTwo);
		// 	}
		// }
	}
}
