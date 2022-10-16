using System;
using Microsoft.Extensions.DependencyInjection;

namespace Amusoft.Toolkit.System.CommandLine.Hosting;

/// <summary>
/// Extensions for <see cref="IServiceCollection"/>
/// </summary>
public static class ServiceCollectionExtensions
{
	/// <summary>
	/// Datasource Configuration for <see cref="IRootCommandProvider"/>
	/// </summary>
	/// <param name="source"></param>
	/// <param name="hierarchy"></param>
	/// <returns></returns>
	public static IServiceCollection AddCommandHierarchy(this IServiceCollection source, Action<CommandHierarchyBuilder> hierarchy)
	{
		var builder = CommandHierarchyBuilder.Create();
		hierarchy(builder);

		return AddCommandHierarchy(source, builder);
	}

	/// <summary>
	/// Datasource Configuration for <see cref="IRootCommandProvider"/>
	/// </summary>
	/// <param name="source"></param>
	/// <param name="hierarchy"></param>
	/// <returns></returns>
	public static IServiceCollection AddCommandHierarchy(this IServiceCollection source, ICommandHierarchy hierarchy)
	{
		CommandRegistrar.Register(source, hierarchy.GetAllRegisteredTypes());
		source.AddSingleton(hierarchy);
		AddRootCommandProvider(source);

		return source;
	}

	private static void AddRootCommandProvider(IServiceCollection source)
	{
		source.AddSingleton<IRootCommandProvider, RootCommandProvider>();
	}
}