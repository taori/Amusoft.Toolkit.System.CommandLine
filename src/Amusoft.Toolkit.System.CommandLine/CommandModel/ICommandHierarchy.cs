using System;
using System.Collections.Generic;

namespace Amusoft.Toolkit.System.CommandLine.CommandModel;

/// <summary>
/// Interface used to build a command hierarchy
/// </summary>
public interface ICommandHierarchy
{
	/// <summary>
	/// Hierarchy data holder
	/// </summary>
	Dictionary<Type, HashSet<Type>> Hierarchy { get; }
}