﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualBasic;

namespace Amusoft.Toolkit.System.CommandLine.Generator.Hierarchy;

[Generator(LanguageNames.CSharp)]
internal class HierarchyGenerator : IIncrementalGenerator
{
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		var childProvider = context.SyntaxProvider.ForAttributeWithMetadataName(Constants.ChildAttributeName, IsCommandClass, GetHierarchicFlow)
			.SelectMany(static (tuples, token) => tuples);
		var parentProvider = context.SyntaxProvider.ForAttributeWithMetadataName(Constants.ParentAttributeName, IsCommandClass, GetHierarchicFlow)
			.SelectMany(static (tuples, token) => tuples);
		
		var mergedHierarchy = childProvider.Collect().Combine(parentProvider.Collect());
		context.RegisterSourceOutput(mergedHierarchy, GenerateHierarchy);
	}

	private void GenerateHierarchy(SourceProductionContext context, (ImmutableArray<(bool root, string parent, string child)> Left, ImmutableArray<(bool root, string parent, string child)> Right) input)
	{
		var result = new HashSet<(bool root, string parent, string child)>(input.Left.Concat(input.Right));
		var roots = new HashSet<string>(result.Where(static d => d.root).Select(d => d.parent));
		var globalHierarchy = result.ToLookup(d => d.parent);
		var hierarchyPerRoot = roots
			.Select(root =>
			{
				var children = new HashSet<(string parent, string child)>();
				AddChildrenRecursive(root, globalHierarchy, children);
				return (root, children);
			}).ToDictionary(d => d.root, d => d.children);
		
		var sb = new StringBuilder();
		AppendSource(sb, hierarchyPerRoot);
		context.AddSource("GeneratedHierarchy.g.cs", sb.ToString());
	}

	private void AppendSource(StringBuilder stringBuilder, Dictionary<string, HashSet<(string parent, string child)>> hierarchyPerRoot)
	{
		/*
		 *	// <auto-generated/>
		 *	using Amusoft.Toolkit.System.CommandLine.CommandModel;
			using Amusoft.Toolkit.System.CommandLine.Generator.UnitTests.TestResources.HierarchyGenerator;

			namespace Amusoft.Toolkit.System.CommandLine.Generator.UnitTests.Samples;

			public static class GeneratedHierarchy
			{
				public static readonly ICommandHierarchy ParentCommand = CommandHierarchyBuilder.FromArray(
					new[,]
					{
						{typeof(MixedChainParent), typeof(MixedChainChild1)},
						{typeof(MixedChainParent), typeof(MixedChainChild2)},
					});
			}
		 */

		if (hierarchyPerRoot.Count == 0)
			return;

		var nsParts = hierarchyPerRoot.Keys.First().Split('.');
		var ns = string.Join(".", nsParts.Take(nsParts.Length - 1).ToArray());

		stringBuilder.AppendLine($$"""
			// <auto-generated/>
			using Amusoft.Toolkit.System.CommandLine.CommandModel;

			namespace {{ns}};

			public static class GeneratedHierarchy
			{
			""");

		foreach (var hierarchy in hierarchyPerRoot)
		{
			stringBuilder.AppendLine(AppendSourceHierarchy(hierarchy));
		}

		stringBuilder.Append("""
			}
			"""

			);
	}

	private string AppendSourceHierarchy(KeyValuePair<string, HashSet<(string parent, string child)>> hierarchy)
	{
		var sb = new StringBuilder();
		/* 
				public static readonly ICommandHierarchy ParentCommand = CommandHierarchyBuilder.FromArray(
					new[,]
					{
						{typeof(MixedChainParent), typeof(MixedChainChild1)},
						{typeof(MixedChainParent), typeof(MixedChainChild2)},
					});
		 */

		var hierarchyName = hierarchy.Key.Split('.').Reverse().Take(1).First();

		sb.AppendLine($$"""
			public static readonly ICommandHierarchy {{hierarchyName}} = 
				CommandHierarchyBuilder.FromArray(new[,]
				{
		""");
		foreach (var tuple in hierarchy.Value)
		{
			sb.AppendLine($"\t\t\t{{typeof({tuple.parent}), typeof({tuple.child})}},");
		}

		sb.Append("""		
				});
		""");

		return sb.ToString();
	}

	private void AddChildrenRecursive(string root, ILookup<string, (bool root, string parent, string child)> globalHierarchy, HashSet<(string parent, string child)> results)
	{
		foreach (var tuple in globalHierarchy[root])
		{
			if (results.Add((tuple.parent, tuple.child)))
			{
				AddChildrenRecursive(tuple.child, globalHierarchy, results);
			}
		}
	}

	private static IEnumerable<(bool root, string parent, string child)> GetHierarchicFlow(GeneratorAttributeSyntaxContext context, CancellationToken cancellationToken)
	{
		if (context.TargetSymbol is INamedTypeSymbol classSymbol)
		{
			var hierarchyAttribute = context.Attributes
				.Select(static d => GetHierarchyData(d))
				.Where(static d => d is not null);

			var isRoot = classSymbol is {BaseType.MetadataName: "RootCommand"};

			foreach (var attributeData in hierarchyAttribute)
			{
				if (attributeData is {Data.ConstructorArguments.Length: > 0} && attributeData.Data.ConstructorArguments[0].Value is INamedTypeSymbol {} firstParameterType)
				{
					if (attributeData.ChildAttribute)
						yield return (isRoot, classSymbol.ToDisplayString(), firstParameterType.ToDisplayString());
					if (attributeData.ParentAttribute)
						yield return (isRoot, firstParameterType.ToDisplayString(), classSymbol.ToDisplayString());
				}
			}
		}
	}

	private static HierarchyData? GetHierarchyData(AttributeData attributeData)
	{
		if (attributeData is {AttributeClass: { } attrClass} && attrClass.ToDisplayString() is {} metadataName)
		{
			if (metadataName is Constants.ChildAttributeName)
				return new HierarchyData(attributeData, true, false);
			if (metadataName is Constants.ParentAttributeName)
				return new HierarchyData(attributeData, false, true);
		}

		return default;
	}

	private record HierarchyData(AttributeData Data, bool ChildAttribute, bool ParentAttribute);

	private static bool IsCommandClass(SyntaxNode node, CancellationToken cancellationToken)
	{
		if (node is ClassDeclarationSyntax classDeclaration)
		{
			if (classDeclaration.BaseList is {Types.Count: > 0} baseList)
			{
				return baseList.Types[0].Type is IdentifierNameSyntax {Identifier.Text: "Command" or "RootCommand"};
			}
		}

		return false;
	}
}