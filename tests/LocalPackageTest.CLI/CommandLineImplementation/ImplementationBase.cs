namespace LocalPackageTest.CLI.CommandLineImplementation;

public abstract class ImplementationBase
{
	public abstract Task<int> Execute(string[] args);
}