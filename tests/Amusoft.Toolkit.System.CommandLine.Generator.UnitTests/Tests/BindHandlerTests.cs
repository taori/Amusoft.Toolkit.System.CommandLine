﻿using System.Collections.Immutable;
using System.CommandLine;
using System.Reflection;
using Amusoft.Toolkit.System.CommandLine.Attributes;
using Amusoft.Toolkit.System.CommandLine.Generator.Analyzers;
using Amusoft.Toolkit.System.CommandLine.Generator.UnitTests.Toolkit;
using Amusoft.Toolkit.System.CommandLine.Generators.ExecuteHandler;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Xunit.Abstractions;
using BindHandlerAnalyzerVerifier = Amusoft.Toolkit.System.CommandLine.Generator.UnitTests.Toolkit.ExtendedAnalyzerVerifier
	<Amusoft.Toolkit.System.CommandLine.Generator.Analyzers.CommandCallsBindHandlerAnalyzer>;

namespace Amusoft.Toolkit.System.CommandLine.Generator.UnitTests.Tests;

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
		
		await BindHandlerAnalyzerVerifier.VerifyAnalyzerAsync(input, Configure);
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
		
		var diagnostic = BindHandlerAnalyzerVerifier.Diagnostic().WithSpan(6, 1, 14, 2).WithArguments("TestCommand");
		await BindHandlerAnalyzerVerifier.VerifyAnalyzerAsync(input, Configure, diagnostic);
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
		
		var diagnostic = BindHandlerAnalyzerVerifier.Diagnostic().WithSpan(6, 1, 14, 2).WithArguments("TestCommand");
		await BindHandlerAnalyzerVerifier.VerifyAnalyzerAsync(input, Configure, diagnostic);
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
		
		await BindHandlerAnalyzerVerifier.VerifyAnalyzerAsync(input, Configure);
	}
}