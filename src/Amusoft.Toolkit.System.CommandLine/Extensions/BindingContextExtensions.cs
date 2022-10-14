using System;
using System.CommandLine.Binding;
using Microsoft.Extensions.DependencyInjection;

namespace Amusoft.Toolkit.System.CommandLine.Extensions;

/// <summary>
/// Extensions for BindingContext
/// </summary>
public static class BindingContextExtensions
{
	/// <summary>
	/// Obtains an instance of requested service using the binding context or the fallback provider if the binding context cannot provide a result
	/// </summary>
	/// <param name="source">bindingContext</param>
	/// <param name="fallbackProvider">fallback serviceProvider</param>
	/// <typeparam name="TService">type of requested service</typeparam>
	/// <returns>service instance</returns>
	public static TService GetRequiredService<TService>(this BindingContext source, IServiceProvider? fallbackProvider)
		where TService : notnull
	{
		if (fallbackProvider is null)
			return source.GetRequiredService<TService>();

		return source.GetService<TService>() ?? fallbackProvider.GetRequiredService<TService>();
	}
}