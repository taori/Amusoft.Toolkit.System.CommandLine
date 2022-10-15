using System;
using System.Reflection;
using Amusoft.Toolkit.System.CommandLine.Logging.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace Amusoft.Toolkit.System.CommandLine.Logging.Extensions;

/// <summary>
/// Extensions for ServiceCollection
/// </summary>
public static class ServiceCollectionExtensions
{
	/// <summary>
	/// Adds runtime log level support
	/// </summary>
	/// <param name="source"></param>
	/// <param name="configure"></param>
	/// <returns></returns>
	public static IServiceCollection AddRuntimeLogLevel(this IServiceCollection source, Action<RuntimeLogLevelOptions>? configure = default)
	{
		if (configure is null)
		{
			configure = _ => { };
		}

		source.TryAddSingleton(new RuntimeLogLevelOptions()
		{
			Namespace = Assembly.GetEntryAssembly()?.GetName().Name,
			DefaultLevel = LogLevel.Information,
		});
		source.Configure(configure);

		return source;
	}
}