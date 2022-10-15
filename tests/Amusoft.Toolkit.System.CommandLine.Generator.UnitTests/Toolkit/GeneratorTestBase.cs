using System.CommandLine;
using System.Reflection;
using Amusoft.Toolkit.System.CommandLine.Attributes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NLog.Fluent;
using Xunit.Abstractions;

namespace Amusoft.Toolkit.System.CommandLine.Generator.UnitTests.Toolkit;

public class GeneratorTestBase : TestBase
{
	public record GeneratorTestResult(Compilation outputCompilation, GeneratorDriverRunResult runResult);

	public GeneratorTestBase(ITestOutputHelper outputHelper, GlobalSetupFixture data) : base(outputHelper, data)
	{
	}
	
	protected GeneratorTestResult GetIncrementalGeneratorResults<TGenerator>(string inputSource, bool assertions = false) where TGenerator : IIncrementalGenerator, new()
	{
		Compilation inputCompilation = CreateCompilation(inputSource); 
		foreach (var d in inputCompilation.GetDiagnostics())
		{
			Log.Info(CSharpDiagnosticFormatter.Instance.Format(d));
		}

		var generator = new TGenerator();
		
		GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);
		
		driver = driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out var outputCompilation, out var diagnostics);
		
		var runResult = driver.GetRunResult();

		return new (outputCompilation, runResult);
	}
	
	protected GeneratorTestResult GetSourceGeneratorResults<TGenerator>(string inputSource, bool assertions = false) where TGenerator : ISourceGenerator, new()
	{
		Compilation inputCompilation = CreateCompilation(inputSource); 
		foreach (var d in inputCompilation.GetDiagnostics())
		{
			Log.Info(CSharpDiagnosticFormatter.Instance.Format(d));
		}

		var generator = new TGenerator();
		
		GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);
		
		driver = driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out var outputCompilation, out var diagnostics);
		
		var runResult = driver.GetRunResult();

		return new (outputCompilation, runResult);
	}

	static IEnumerable<string> GetReferenceAssemblyPaths(Assembly assembly)
	{
		var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
		var referencedAssemblies = assembly.GetReferencedAssemblies();
		return referencedAssemblies
			.Select(name => loadedAssemblies.SingleOrDefault(a => a.FullName == name.FullName)?.Location)
			.Where(location => location != null);
	}

	static IEnumerable<MetadataReference> GetMetadataReferences()
	{
		var referenceSources = new[] {typeof(GenerateCommandHandlerAttribute).Assembly};
		foreach (var path in referenceSources.SelectMany(GetReferenceAssemblyPaths) )
		{
			// yield return MetadataReference.CreateFromFile(path);
		}

		yield return MetadataReference.CreateFromFile(Assembly.Load("System.Runtime, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a").Location);
		yield return MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location);
		yield return MetadataReference.CreateFromFile(Assembly.Load("netstandard, Version=2.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51").Location);
		yield return MetadataReference.CreateFromFile(typeof(Enumerable).GetTypeInfo().Assembly.Location);
		yield return MetadataReference.CreateFromFile(typeof(Command).GetTypeInfo().Assembly.Location);
		yield return MetadataReference.CreateFromFile(typeof(Binder).GetTypeInfo().Assembly.Location);
		yield return MetadataReference.CreateFromFile(typeof(GenerateCommandHandlerAttribute).GetTypeInfo().Assembly.Location);
	}

	private static Compilation CreateCompilation(string source)
		=> CSharpCompilation.Create("compilation",
			new[] { CSharpSyntaxTree.ParseText(source) },
			GetMetadataReferences().ToArray(),
			new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
}