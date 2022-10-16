using System;
using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;

namespace Amusoft.Toolkit.System.CommandLine.Hosting;

internal class RootCommandProvider : IRootCommandProvider
{
	private readonly ICommandHierarchy _commandHierarchy;
	private readonly IServiceProvider _serviceProvider;

	public RootCommandProvider(ICommandHierarchy commandHierarchy, IServiceProvider serviceProvider)
	{
		_commandHierarchy = commandHierarchy;
		_serviceProvider = serviceProvider;
	}

	private bool _commandTreeBuilt;

	public TCommand GetCommand<TCommand>() where TCommand : RootCommand
	{
		if (_commandTreeBuilt)
			return _serviceProvider.GetRequiredService<TCommand>();

		var rootCommand = _serviceProvider.GetRequiredService<TCommand>();
		if (rootCommand is null)
			throw new Exception($"The type {typeof(TCommand).FullName} was not registered");

		AppendChildCommands(rootCommand);

		_commandTreeBuilt = true;
		return rootCommand;
	}

	private void AppendChildCommands(Command command)
	{
		var sourceType = command.GetType();
		if (_commandHierarchy.Hierarchy.TryGetValue(sourceType, out var childTypes))
		{
			foreach (var childType in childTypes)
			{
				var childCommand = _serviceProvider.GetRequiredService(childType) as Command;
				if (childCommand is null)
					throw new Exception($"{childType} does not appear to be a command. Command tree can not be built");

				command.AddCommand(childCommand);

				AppendChildCommands(childCommand);
			}
		}
	}
}