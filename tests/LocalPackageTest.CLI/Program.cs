using LocalPackageTest.CLI.CommandLineImplementation;

namespace LocalPackageTest.CLI
{
	internal class Program
	{
		static Task<int> Main(string[] args)
		{
			var model = new HostingModel();
			return model.Execute(args);
		}
	}
}