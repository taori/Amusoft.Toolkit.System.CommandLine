﻿//HintName: MixedCommand.g.cs
// <auto-generated/>

using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;
using Amusoft.Toolkit.System.CommandLine.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Amusoft.Toolkit.System.CommandLine.UnitTests.TestResources.ExecuteHandlerGenerator;	

internal partial class MixedCommand : MixedCommand.ICommandInvoker
{		
	private void BindHandler()
	{
		this.SetHandler(async (context) =>
		{
			var p1 = context.ParseResult.GetValueForOption(TestOption);
			var p2 = context.ParseResult.GetValueForArgument(TestArgument);
			
			await ExecuteAsync(context, p1, p2);
		});
	}

	internal interface ICommandInvoker
	{
		protected abstract Task ExecuteAsync(InvocationContext context, string testOption, string testArgument);
	}
}