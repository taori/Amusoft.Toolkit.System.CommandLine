using System;

namespace Amusoft.Toolkit.System.CommandLine.Attributes;

/// <summary>
/// This class is used to fuel a generator with metadata about hierarchic dependencies
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public class HasChildCommandAttribute : Attribute
{
	/// <summary>
	/// Constructor used to indicate the child command of the current type
	/// </summary>
	/// <param name="target">type of the child command</param>
	public HasChildCommandAttribute(Type target)
	{
		Target = target;
	}
	
	/// <summary>
	/// Type of the child command
	/// </summary>
	public Type Target { get; }
}

/// <summary>
/// This class is used to fuel a generator with metadata about hierarchic dependencies
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class HasParentCommandAttribute : Attribute
{
	/// <summary>
	/// Constructor used to indicate the parent command of the current type
	/// </summary>
	/// <param name="target">type of the parent command</param>
	public HasParentCommandAttribute(Type target)
	{
		Target = target;
	}

	/// <summary>
	/// Type of the parent command
	/// </summary>
	public Type Target { get; }
}