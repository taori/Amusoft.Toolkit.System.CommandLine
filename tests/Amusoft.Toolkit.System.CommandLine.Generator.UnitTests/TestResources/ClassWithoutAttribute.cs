using System.CommandLine;

namespace Amusoft.Toolkit.System.CommandLine.Generator.UnitTests.TestResources;

public partial class ClassWithoutAttribute : Command
{
	public ClassWithoutAttribute(string name, string description = null) : base(name, description)
	{
	}
}