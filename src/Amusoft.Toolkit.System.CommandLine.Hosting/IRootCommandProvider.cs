using System.CommandLine;

namespace Amusoft.Toolkit.System.CommandLine.Hosting;

/// <summary>
/// Provider interface used to identify the root of commands
/// </summary>
public interface IRootCommandProvider
{
	/// <summary>
	/// Obtains instance of constructed command
	/// </summary>
	/// <typeparam name="TCommand"></typeparam>
	/// <returns></returns>
	TCommand GetCommand<TCommand>() where TCommand : RootCommand;
}