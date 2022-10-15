using System;
using System.CommandLine.Binding;
using System.CommandLine.Help;
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
	public static TService GetRequiredServiceWithFallback<TService>(this BindingContext source, IServiceProvider? fallbackProvider)
		where TService : notnull
	{
		if (source.GetService<TService>() is { } result)
			return result;

		if (fallbackProvider is not null && fallbackProvider.GetService<TService>() is { } fallbackService)
			return fallbackService;

		if (source.GetService<IServiceProvider>() is { } subServiceProvider && subServiceProvider.GetRequiredService<TService>() is {} subService)
			return subService;

		throw new Exception($"Service {typeof(TService).FullName} not found");
	}
}