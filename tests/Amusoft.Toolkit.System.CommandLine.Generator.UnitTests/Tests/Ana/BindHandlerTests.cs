using System.Collections.Immutable;
using System.CommandLine;
using System.Reflection;
using Amusoft.Toolkit.System.CommandLine.Attributes;
using Amusoft.Toolkit.System.CommandLine.Generator.Analyzers;
using Amusoft.Toolkit.System.CommandLine.Generator.UnitTests.Toolkit;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Xunit.Abstractions;
using BindHandlerAnalyzerVerifier = Amusoft.Toolkit.System.CommandLine.Generator.UnitTests.Toolkit.ExtendedAnalyzerVerifier
    <Amusoft.Toolkit.System.CommandLine.Generator.Analyzers.CommandCallsBindHandlerAnalyzer>;

namespace Amusoft.Toolkit.System.CommandLine.Generator.UnitTests.Tests.Ana;

public class BindHandlerTests : AnalyzerTestBase<CommandCallsBindHandlerAnalyzer>
{
    public BindHandlerTests(ITestOutputHelper outputHelper, GlobalSetupFixture data) : base(outputHelper, data)
    {
    }

    [Fact]
    async Task NoDiagnosticEverythingOkay()
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
				BindHandler();
			}

			private void BindHandler() { }
		}
		""";

        await RunTestAsync(input, test =>
        {
            test.DisabledDiagnostics.Add("ATSCG003");
        });
    }

    [Fact]
    async Task BindCallMissing()
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

        var diagnostic = BindHandlerAnalyzerVerifier.Diagnostic("001").WithSpan(7, 22, 7, 33).WithArguments("TestCommand");
        await RunTestAsync(input, test =>
        {
            test.DisabledDiagnostics.Add("ATSCG003");
        }, diagnostic);
    }

    [Fact]
    async Task AttributeMissingWithDiagnostic()
    {
        var input = """
		using System.CommandLine;
		using Amusoft.Toolkit.System.CommandLine.Attributes;

		namespace Amusoft.Toolkit.System.CommandLine.Generator.UnitTests.Tests;	
		
		public partial class TestCommand : Command
		{
			public TestCommand() : base("")
			{
			}

			private void BindHandler() { }
		}
		""";

        var diagnostic = BindHandlerAnalyzerVerifier.Diagnostic("002").WithSpan(6, 22, 6, 33).WithArguments("TestCommand");
        await RunTestAsync(input, test =>
        {
            test.DisabledDiagnostics.Add("ATSCG003");
        }, diagnostic);
    }

    [Fact]
    async Task AttributeMissingWithoutDiagnostic()
    {
        var input = """
		using System.CommandLine;
		using Amusoft.Toolkit.System.CommandLine.Attributes;

		namespace Amusoft.Toolkit.System.CommandLine.Generator.UnitTests.Tests;		
		
		public partial class TestCommand 
		{
			public TestCommand() 
			{
			}

			private void BindHandler() { }
		}
		""";

        await RunTestAsync(input, test =>
        {
            test.DisabledDiagnostics.Add("ATSCG003");
        });
    }

    [Fact]
    async Task NoRootCommandWithDiagnostic()
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
				BindHandler();
			}

			private void BindHandler() { }
		}
		""";

        var diagnostic = new DiagnosticResult(CommandCallsBindHandlerAnalyzer.RootCommandMustBeInheritedRule);
        await RunTestAsync(input, diagnostic);
    }

    [Fact]
    async Task NoRootCommandWithoutDiagnostic()
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
				BindHandler();
			}

			private void BindHandler() { }
		}	

		
		public partial class ApplicationRootCommand : RootCommand
		{
			public ApplicationRootCommand() : base("")
			{
			}
		}
		""";

        await RunTestAsync(input);
    }

    [Fact]
    async Task CommandWithDelegationNoDiagnostic()
    {
        var input = """
		using System.CommandLine;
		using Amusoft.Toolkit.System.CommandLine.Attributes;
		
		namespace Amusoft.Toolkit.System.CommandLine.Generator.UnitTests.Tests;		
		
		[HasChildCommand(typeof(TestCommand2))]
		public class TestCommand : Command
		{
			public TestCommand() : base("")
			{
			}
		}	
		
		[HasParentCommand(typeof(TestCommand))]
		public class TestCommand2 : Command
		{
			public TestCommand2() : base("")
			{
			}
		}	
		""";

        await RunTestAsync(input, config =>
        {
            config.DisabledDiagnostics.Add("ATSCG003");
        });
    }
}