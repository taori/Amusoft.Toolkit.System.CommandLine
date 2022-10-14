using System.CommandLine;
using Amusoft.Toolkit.System.CommandLine.Attributes;

namespace Amusoft.Toolkit.System.CommandLine.Generator.UnitTests.TestResources.ExecuteHandlerGenerator;

[GenerateExecuteHandler]
internal partial class MixedCommandNoParameter : Command
{
	public MixedCommandNoParameter() : base("sub1", "sub1 description")
	{
	}
}