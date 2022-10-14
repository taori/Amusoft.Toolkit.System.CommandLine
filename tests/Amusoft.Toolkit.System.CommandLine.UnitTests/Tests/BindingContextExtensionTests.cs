using System;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using Amusoft.Toolkit.System.CommandLine.Extensions;
using Amusoft.Toolkit.System.CommandLine.UnitTests.Toolkit;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace Amusoft.Toolkit.System.CommandLine.UnitTests.Tests;

public class BindingContextExtensionTests : TestBase
{
	public BindingContextExtensionTests(ITestOutputHelper outputHelper, GlobalSetupFixture data) : base(outputHelper, data)
	{
	}

	[Fact]
	public void VerifyNoFallback()
	{
		var clb = new CommandLineBuilder();
		Assert.Throws<InvalidOperationException>(() =>
		{
			clb.AddMiddleware(context =>
			{
				var service = context.BindingContext.GetRequiredService<Func<string>>(null);
			}).Build().Invoke(new string[]{"5"});
		});
	}

	[Fact]
	public void VerifyFallback()
	{
		var serviceCollection = new ServiceCollection();
		serviceCollection.AddSingleton<Func<string>>(() => "test");
		var clb = new CommandLineBuilder();
		Assert.Throws<InvalidOperationException>(() =>
		{
			clb.AddMiddleware(context =>
			{
				var service = context.BindingContext.GetRequiredService<Func<string>>(null);
				service().ShouldBe("test");
			}).Build().Invoke(new string[]{"5"});
		});
	}
}