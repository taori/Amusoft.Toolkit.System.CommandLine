using System;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.Parsing;
using System.IO;
using System.Reflection;
using Amusoft.Toolkit.System.CommandLine.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Amusoft.Toolkit.System.CommandLine.Hosting;

/// <summary>
/// Helper class to configure an underlying <see cref="CommandLineBuilder"/>
/// </summary>
public class HostedRootCommandBuilder
{
	private IServiceCollection _serviceCollection = new ServiceCollection();
	private IConfiguration? _configuration;
	private Action<IConfiguration, IServiceCollection>? _configureServicesCallback;
	private Action<ConsoleApplicationCreationContext, CommandLineBuilder>? _defaultCommandLineBuilder;

	/// <summary>
	/// Constructor method
	/// </summary>
	/// <returns></returns>
	public static HostedRootCommandBuilder Create() => new();

	internal HostedRootCommandBuilder()
	{
	}

	/// <summary>
	/// Uses the configuration from a parameter
	/// </summary>
	/// <param name="configuration"></param>
	/// <returns></returns>
	public HostedRootCommandBuilder UseConfiguration(IConfiguration configuration)
	{
		_configuration = configuration;
		return this;
	}

	/// <summary>
	/// Configures the configuration as specified
	/// </summary>
	/// <param name="configure"></param>
	/// <returns></returns>
	public HostedRootCommandBuilder UseConfiguration(Action<ConfigurationBuilder> configure)
	{
		var builder = new ConfigurationBuilder();
		configure?.Invoke(builder);
		UseConfiguration(builder.Build());
		return this;
	}

	/// <summary>
	/// Uses the default specification while setting up the base path and adding the default appsettings.json sources
	/// </summary>
	/// <param name="environment"></param>
	/// <returns></returns>
	public HostedRootCommandBuilder UseDefaultConfiguration(string? environment = default)
	{
		environment ??= "Production";

		UseConfiguration(builder =>
		{
			builder
				.SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
				.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
				.AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true);
		});

		return this;
	}

	/// <summary>
	/// Specified service configuration for the bindingContext and the outer serviceProvider
	/// </summary>
	/// <param name="configuration"></param>
	/// <returns></returns>
	public HostedRootCommandBuilder ConfigureServices(Action<IConfiguration, IServiceCollection> configuration)
	{
		if (configuration == null)
			return this;

		_configureServicesCallback = configuration;
		return this;
	}

	/// <summary>
	/// Configures the <see cref="CommandLineBuilder"/> using the default configuration
	/// </summary>
	/// <example>
	///		<code>
	///			builder
	///				.UseServiceProviderFallback(context.ServiceProvider)
	///				.UseHost(args =>
	///				{
	///					return Host.CreateDefaultBuilder(args)
	///						.ConfigureServices(hostContext =>
	///						{
	///							_configureServicesCallback?.Invoke(_configuration, hostContext);
	///						});
	///				})
	///				.UseDefaults();
	///		</code>
	/// </example>
	/// <returns></returns>
	public HostedRootCommandBuilder UseDefaultCommandLineBuilder()
	{
		_defaultCommandLineBuilder = (context, builder) =>
		{
			if (_configuration is null)
				throw new Exception($"Configuration must be set up first");

			builder
				.UseServiceProviderFallback(context.ServiceProvider)
				.UseHost(args =>
				{
					return Host.CreateDefaultBuilder(args)
						.ConfigureServices(hostContext =>
						{
							_configureServicesCallback?.Invoke(_configuration, hostContext);
						});
				})
				.UseDefaults();
		};

		return this;
	}

	/// <summary>
	/// Builds the application which should be invoked afterwards
	/// </summary>
	/// <param name="configure"></param>
	/// <returns></returns>
	public Parser Build(Action<ConsoleApplicationCreationContext, CommandLineBuilder> configure)
	{
		if (_configuration is null)
			throw new Exception("Configuration is not set up yet");

		_configureServicesCallback?.Invoke(_configuration, _serviceCollection);

		var context = new ConsoleApplicationCreationContext(_configuration, _serviceCollection.BuildServiceProvider());

		var rootCommandProvider = context.ServiceProvider.GetRequiredService<IRootCommandProvider>();
		var rootCommand = rootCommandProvider.GetCommand();
		var commandLineBuilder = new CommandLineBuilder(rootCommand);
		
		_defaultCommandLineBuilder?.Invoke(context, commandLineBuilder);
		configure(context, commandLineBuilder);

		return commandLineBuilder.Build();
	}
}