﻿//HintName: ClassWithAttribute.g.cs
// <auto-generated/>
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;
using Amusoft.Toolkit.System.CommandLine.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Amusoft.Toolkit.System.CommandLine.UnitTests.TestResources;	

public partial class ClassWithAttribute
{
	private ClassWithHandler _handler;

	private void BindHandler()
	{
		if (_handler is not null)
			return;

		this.SetHandler(async (context) =>
		{
			var host = context.BindingContext.GetRequiredService<IHost>();
			_handler = new ClassWithHandler(host.Services.GetRequiredService<string>());
			var p1 = context.ParseResult.GetValueForOption(OptionOne);
			var p2 = context.ParseResult.GetValueForOption(OptionTwo);
			var p3 = context.ParseResult.GetValueForArgument(ArgumentOne);
			await _handler.ExecuteAsync(context, p1, p2, p3);
		});
	}

	public partial class ClassWithHandler : InvokerBase
	{
	}

	public abstract class InvokerBase
	{
		public virtual Task ExecuteAsync(InvocationContext context, string optionOne, int optionTwo, int argumentOne) => Task.CompletedTask;	
	}
}