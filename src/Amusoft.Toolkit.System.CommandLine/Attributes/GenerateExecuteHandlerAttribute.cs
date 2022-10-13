using System;

namespace Amusoft.Toolkit.System.CommandLine.Attributes;

/// <summary>
/// Marker attribute for classes which should have an Execute handler generated
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class GenerateExecuteHandlerAttribute : Attribute
{
	
}