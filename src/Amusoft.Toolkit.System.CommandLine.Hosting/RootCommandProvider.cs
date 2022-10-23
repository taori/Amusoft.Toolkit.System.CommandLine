using System;
using System.CommandLine;
using System.Linq;
using Amusoft.Toolkit.System.CommandLine.CommandModel;
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
	private Type? _rootCommandType;

	public RootCommand? GetCommand()
	{
		if (_commandTreeBuilt && _rootCommandType is not null)
			return _serviceProvider.GetRequiredService(_rootCommandType) as RootCommand;

		var rootCommandType = FindRootCommandType(_commandHierarchy);
		if (rootCommandType is null)
			throw new Exception("There is no command of type RootCommand in the command hierarchy");

		var rootCommand = _serviceProvider.GetRequiredService(rootCommandType) as RootCommand;
		if (rootCommand is null)
			throw new Exception($"The type {rootCommandType.FullName} was not registered");

		AppendChildCommands(rootCommand);

		_rootCommandType = rootCommandType;
		_commandTreeBuilt = true;
		return rootCommand;
	}

	private Type? FindRootCommandType(ICommandHierarchy commandHierarchy)
	{
		return commandHierarchy.Hierarchy.Keys.FirstOrDefault(d => typeof(RootCommand).IsAssignableFrom(d));
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