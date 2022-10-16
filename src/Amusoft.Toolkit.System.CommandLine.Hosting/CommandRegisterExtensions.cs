using System;
using System.Collections.Generic;
using System.Linq;
using Amusoft.Toolkit.System.CommandLine.CommandModel;

namespace Amusoft.Toolkit.System.CommandLine.Hosting;

internal static class CommandRegisterExtensions
{
	public static Type[] GetAllRegisteredTypes(this ICommandHierarchy source)
	{
		return new HashSet<Type>(
				source.Hierarchy.SelectMany(d => d.Value)
					.Concat(source.Hierarchy.Select(d => d.Key)))
			.ToArray();
	}
}