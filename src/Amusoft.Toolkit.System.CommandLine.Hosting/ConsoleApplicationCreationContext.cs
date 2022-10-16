using System;
using Microsoft.Extensions.Configuration;

namespace Amusoft.Toolkit.System.CommandLine.Hosting;

/// <summary>
/// Creation context for application
/// </summary>
public class ConsoleApplicationCreationContext
{
	internal ConsoleApplicationCreationContext(IConfiguration configuration, IServiceProvider serviceProvider)
	{
		Configuration = configuration;
		ServiceProvider = serviceProvider;
	}

	/// <summary>
	/// Previously configured configuration object
	/// </summary>
	public IConfiguration Configuration { get; }

	/// <summary>
	/// Result of configured services
	/// </summary>
	public IServiceProvider ServiceProvider { get; }
}