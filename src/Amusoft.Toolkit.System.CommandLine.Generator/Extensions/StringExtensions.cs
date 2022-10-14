namespace Amusoft.Toolkit.System.CommandLine.Generator.Extensions;

internal static class StringExtension
{
	public static string ToCamelCase(this string source)
	{
		if (string.IsNullOrEmpty(source))
			return source;
		if (source.Length <= 1)
			return source.ToLowerInvariant();

		return char.ToLowerInvariant(source[0]) + source.Substring(1);
	}
}