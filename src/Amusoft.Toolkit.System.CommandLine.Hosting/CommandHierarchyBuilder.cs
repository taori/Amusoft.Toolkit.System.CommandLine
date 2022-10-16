using System;
using System.Collections.Generic;
using System.Linq;

namespace Amusoft.Toolkit.System.CommandLine.Hosting;

/// <summary>
/// Data factory to hold hierarchic information about the command structure
/// </summary>
public class CommandHierarchyBuilder : ICommandHierarchy
{
	internal Dictionary<Type, HashSet<Type>> Hierarchy { get; } = new();

	Dictionary<Type, HashSet<Type>> ICommandHierarchy.Hierarchy => this.Hierarchy;

	internal CommandHierarchyBuilder()
	{
	}

	/// <summary>
	/// Builds CommandHierarchy from parent->child array
	/// </summary>
	/// <param name="values"></param>
	/// <returns></returns>
	public static CommandHierarchyBuilder FromArray(Type[,] values)
	{
		var builder = new CommandHierarchyBuilder();
		if(values.GetLength(1) != 2)
			throw new Exception($"child array must be of size 2");

		var rowLength = values.GetLength(0);
		for (int rowIndex = 0; rowIndex < rowLength; rowIndex++)
		{
			builder.AddMapping(values[rowIndex, 0], values[rowIndex, 1]);
		}

		return builder;
	}

	/// <summary>
	/// Creates an instance of the builder
	/// </summary>
	/// <returns></returns>
	public static CommandHierarchyBuilder Create() => new ();

	/// <summary>
	/// Adds a mapping to the register
	/// </summary>
	/// <param name="parent">parent command type</param>
	/// <param name="child">child command type</param>
	/// <returns></returns>
	public CommandHierarchyBuilder AddMapping(Type parent, Type child)
	{
		if (Hierarchy.TryGetValue(parent, out var children))
		{
			children.Add(child);
		}
		else
		{
			children = new HashSet<Type>();
			children.Add(child);
			Hierarchy.Add(parent, children);
		}

		return this;
	}

	internal Type[] GetAllRegisteredTypes()
	{
		return new HashSet<Type>(
				Hierarchy.SelectMany(d => d.Value)
					.Concat(Hierarchy.Select(d => d.Key)))
			.ToArray();
	}
}