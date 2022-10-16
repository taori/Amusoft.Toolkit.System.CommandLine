using System;
using Microsoft.Extensions.DependencyInjection;

namespace Amusoft.Toolkit.System.CommandLine.Hosting;

internal static class CommandRegistrar
{
	public static void Register(IServiceCollection serviceCollection, Type[] types)
	{
		foreach (var type in types)
		{
			serviceCollection.AddSingleton(type);
		}
	}
}