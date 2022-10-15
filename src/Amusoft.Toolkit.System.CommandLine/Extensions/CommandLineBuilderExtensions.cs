using System.CommandLine.Builder;
using System;

namespace Amusoft.Toolkit.System.CommandLine.Extensions;

/// <summary>
/// Extensions for <see cref="CommandLineBuilder"/>
/// </summary>
public static class CommandLineBuilderExtensions
{

	/// <summary>
	/// Adds an external serviceProvider to the BindingContext
	/// </summary>
	/// <param name="source"></param>
	/// <param name="serviceProvider">service provider instance to fall back to</param>
	/// <returns></returns>
	public static CommandLineBuilder UseServiceProviderFallback(this CommandLineBuilder source, IServiceProvider serviceProvider)
	{
		source.AddMiddleware(context =>
		{
			context.BindingContext.AddService(typeof(IServiceProvider), _ => serviceProvider);
		});

		return source;
	}

}