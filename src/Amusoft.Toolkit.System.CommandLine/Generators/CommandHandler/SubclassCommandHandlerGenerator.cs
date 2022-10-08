﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Amusoft.Toolkit.System.CommandLine.Attributes;
using Amusoft.Toolkit.System.CommandLine.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Amusoft.Toolkit.System.CommandLine.Generators.CommandHandler;

[Generator]
internal class SubclassCommandHandlerGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
    }

    public void Execute(GeneratorExecutionContext context)
    {
        foreach (var syntaxTree in context.Compilation.SyntaxTrees)
        {
            var root = syntaxTree.GetRoot(context.CancellationToken);
            foreach (var outerClass in ClassesWhichImplementCommand(root))
			{
				var semanticModel = context.Compilation.GetSemanticModel(syntaxTree);
				if (TryGetCandidate(semanticModel, outerClass, out var innerClass))
                {
                    AppendGeneratorCode(context, semanticModel, outerClass, innerClass);
                }
            }
        }
    }

    private static IEnumerable<ClassDeclarationSyntax> ClassesWhichImplementCommand(SyntaxNode root)
    {
        return root.DescendantNodes(_ => true).OfType<ClassDeclarationSyntax>()
            .Where(classSyntax => classSyntax
                .ChildNodes()
                .OfType<ClassDeclarationSyntax>()
                .Where(subClass => ClassHasHandlerAttributeSyntax(subClass))
                .Any());
    }

    private static bool ClassHasHandlerAttributeSyntax(ClassDeclarationSyntax subClass)
    {
	    return subClass.AttributeLists.Any(attributeListSyntax => 
		    attributeListSyntax.Attributes.Any(attribute => attribute.Name is IdentifierNameSyntax { Identifier.Text: "GenerateCommandHandler" or "GenerateCommandHandlerAttribute" }));
    }

    private void AppendGeneratorCode(GeneratorExecutionContext context, SemanticModel semanticModel, ClassDeclarationSyntax outerClass, ClassDeclarationSyntax innerClass)
    {
        if (outerClass == null) throw new ArgumentNullException(nameof(outerClass));
        if (innerClass == null) throw new ArgumentNullException(nameof(innerClass));

        var outerClassSymbol = semanticModel.GetDeclaredSymbol(outerClass);
        if (outerClassSymbol is null)
            return;

        var targetNamespace = outerClassSymbol.ContainingNamespace.ToDisplayString();
        var constructorServiceList = GetConstructorServices(semanticModel, innerClass)
            .Select(d => $"host.Services.GetRequiredService<{d}>()");
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
		
		{{outerClass.Modifiers}} class {{outerClass.Identifier}}
		{
			private {{innerClass.Identifier.Text}} _handler;

			private void BindHandler()
			{
				if (_handler is not null)
					return;

				this.SetHandler(async (context) =>
				{
					var host = context.BindingContext.GetRequiredService<IHost>();
					_handler = new {{innerClass.Identifier.Text}}({{string.Join(",", constructorServiceList)}});
	
		""");

        var commandParameterDeclarations = GetCommandParameterDeclarations(semanticModel, outerClass).ToArray();
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
					await _handler.ExecuteAsync(context);
	
		""");
        }
        else
        {
            sb.Append(
        $$"""
					await _handler.ExecuteAsync(context, {{string.Join(", ", commandParameterDeclarations.Select(d => d.InvokeName))}});
	
		""");
        }

        sb.Append(
        $$"""
				});
			}

			{{innerClass.Modifiers}} class {{innerClass.Identifier.Text}} : InvokerBase
			{
			}
		
			public abstract class InvokerBase
			{
				public virtual Task ExecuteAsync({{GetInvokerBaseParameterList(commandParameterDeclarations)}}) => Task.CompletedTask;	
			}
		}
		""");

        context.AddSource($"{outerClass.Identifier.Text}.g.cs", sb.ToString());
    }

    private static string GetInvokerBaseParameterList(ParameterDeclaration[] parameters)
    {
        if (parameters.Length == 0)
            return "InvocationContext context";

        var sb = new StringBuilder("InvocationContext context");
        for (int i = 0; i < parameters.Length; i++)
        {
            var parameter = parameters[i];
            sb.Append($", {parameter.UnwrappedOptionType} {parameter.OptionName.ToCamelCase()}");
        }

        return sb.ToString();
    }

    private record ParameterDeclaration(string InvokeName, string OptionName, string ParseResultCallName, string UnwrappedOptionType);

    private IEnumerable<ParameterDeclaration> GetCommandParameterDeclarations(SemanticModel semanticModel, ClassDeclarationSyntax outerClass)
    {
        var index = 1;
        var optionType = semanticModel.Compilation.GetTypeByMetadataName("System.CommandLine.Option`1");
        var argumentType = semanticModel.Compilation.GetTypeByMetadataName("System.CommandLine.Argument`1");

        foreach (var memberDeclarationSyntax in outerClass.Members)
        {
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

    private bool IsPropertyOfQueryType(ISymbol propertyTypeSymbol, INamedTypeSymbol? propertyQueryType, [NotNullWhen(true)] out string? unwrappedQueryType)
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

    private string[] GetConstructorServices(SemanticModel semanticModel, ClassDeclarationSyntax innerClass)
    {
        var constructorSyntax = innerClass.ChildNodes().OfType<ConstructorDeclarationSyntax>()
            .FirstOrDefault();
        if (constructorSyntax is null)
            return Array.Empty<string>();

        if (constructorSyntax.ParameterList is { Parameters: { } parameters })
        {
            var parameterTypes = parameters.Select(parameter =>
            {
                if (parameter.Type != null && semanticModel.GetSymbolInfo(parameter.Type) is { Symbol: { } symbol })
                    return $"{symbol.ToDisplayString()}";

                return string.Empty;
            });

            return parameterTypes.ToArray();
        }

        return Array.Empty<string>();
    }

    private static bool TryGetCandidate(SemanticModel semanticModel, ClassDeclarationSyntax parentClass, [NotNullWhen(true)] out ClassDeclarationSyntax? implementorChildClass)
    {
        implementorChildClass = default;
        if (parentClass.BaseList is null)
            return false;

        var parentSymbol = semanticModel.GetDeclaredSymbol(parentClass);
        var handlerAttributeTypeSymbol = semanticModel.Compilation.GetTypeByMetadataName(typeof(GenerateCommandHandlerAttribute).FullName!);
        var commandSymbol = semanticModel.Compilation.GetTypeByMetadataName("System.CommandLine.Command");

        var inheritsCommand = parentClass.BaseList.Types.Any(baseType =>
        {
            if (semanticModel.GetSymbolInfo(baseType.Type) is { Symbol: { } symbol })
                return symbol.Equals(commandSymbol, SymbolEqualityComparer.Default);
            return false;
        });

        if (!inheritsCommand)
            return false;

        implementorChildClass = parentClass.ChildNodes()
            .OfType<ClassDeclarationSyntax>()
            .Where(syntax =>
            {
                var classSymbol = semanticModel.GetDeclaredSymbol(syntax);
                if (classSymbol is null)
                    return false;

                return classSymbol.GetAttributes().Any(attributeData =>
                {
                    if (attributeData is { AttributeClass: { } attributeClass })
                        return attributeClass.Equals(handlerAttributeTypeSymbol, SymbolEqualityComparer.Default);
                    return true;
                });
            })
            .FirstOrDefault();

        return implementorChildClass != null;
    }
}