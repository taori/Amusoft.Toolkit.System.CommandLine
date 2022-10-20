using Amusoft.Toolkit.System.CommandLine.Generator.Analyzers;
using Amusoft.Toolkit.System.CommandLine.Generator.Codefixes;
using Amusoft.Toolkit.System.CommandLine.Generator.UnitTests.Toolkit;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit.Abstractions;

namespace Amusoft.Toolkit.System.CommandLine.Generator.UnitTests.Tests.Fix;

#pragma warning disable CS1591

public class BindHandlerTests : CodeFixTestBase<CommandCallsBindHandlerAnalyzer, BindHandlerCodeFixProvider>
{
	public BindHandlerTests(ITestOutputHelper outputHelper) : base(outputHelper)
	{
	}

	[Fact]
	public async Task VerifyConstructorEmpty()
	{
		var input = """
		using System.CommandLine;
		using Amusoft.Toolkit.System.CommandLine.Attributes;
		
		namespace Amusoft.Toolkit.System.CommandLine.Generator.UnitTests.Tests;		
		
		[GenerateExecuteHandler]
		public partial class TestCommand : Command
		{
			public TestCommand() : base("")
			{
			}

			private void BindHandler() { }
		}
		""";

		var output = """
		using System.CommandLine;
		using Amusoft.Toolkit.System.CommandLine.Attributes;
		
		namespace Amusoft.Toolkit.System.CommandLine.Generator.UnitTests.Tests;		
		
		[GenerateExecuteHandler]
		public partial class TestCommand : Command
		{
			public TestCommand() : base("")
			{
		        BindHandler();
		    }

			private void BindHandler() { }
		}
		""";
		
		var diagnostic = new[]
		{
			DiagnosticById("ATSCG001").WithSpan(7, 22, 7, 33).WithArguments("TestCommand"),
		};
		var test = CreateTest(input, diagnostic, output);
		test.DisabledDiagnostics.Add("ATSCG003");
		// test.MarkupOptions
		// test.OptionsTransforms
		// test.SolutionTransforms
		await test.RunAsync();
	}

	[Fact]
	public async Task VerifyConstructorNonEmpty()
	{
		var input = """
		using System.CommandLine;
		using Amusoft.Toolkit.System.CommandLine.Attributes;
		
		namespace Amusoft.Toolkit.System.CommandLine.Generator.UnitTests.Tests;		
		
		[GenerateExecuteHandler]
		public partial class TestCommand : Command
		{
			public TestCommand() : base("")
			{
				OtherMethod();
			}

			private void BindHandler() { }
			private void OtherMethod() { }
		}
		""";
		var output = """
		using System.CommandLine;
		using Amusoft.Toolkit.System.CommandLine.Attributes;
		
		namespace Amusoft.Toolkit.System.CommandLine.Generator.UnitTests.Tests;		
		
		[GenerateExecuteHandler]
		public partial class TestCommand : Command
		{
			public TestCommand() : base("")
			{
				OtherMethod();
		        BindHandler();
		    }

			private void BindHandler() { }
			private void OtherMethod() { }
		}
		""";

		var diagnostic = new[]
		{
			DiagnosticById("ATSCG001").WithSpan(7, 22, 7, 33).WithArguments("TestCommand"),
		};
		var test = CreateTest(input, diagnostic, output);
		test.DisabledDiagnostics.Add("ATSCG003");
		await test.RunAsync();
	}
}