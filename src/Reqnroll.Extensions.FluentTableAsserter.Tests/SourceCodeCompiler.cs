using System.Reflection;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Reqnroll.Extensions.FluentTableAsserter.CollectionAsserters;

namespace Reqnroll.Extensions.FluentTableAsserter.Tests;

public static class SourceCodeCompiler
{
    public static IEnumerable<string> Compile(params string[] sourceCodes)
    {
        sourceCodes
            .Should()
            .AllSatisfy(x => x.Should().NotBeNullOrEmpty("without source code there is nothing to test"));

        var trees = sourceCodes
            .Select(x => x.Trim())
            .Select(x => CSharpSyntaxTree.ParseText(x))
            .ToArray();

        Assembly GetAssembly(string assemblyName)
        {
            return AppDomain.CurrentDomain
                .GetAssemblies()
                .Single(a => a.GetName().Name == assemblyName);
        }

        var compilation = CSharpCompilation.Create("Test")
            .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
            .AddReferences(
                MetadataReference.CreateFromFile(GetAssembly("netstandard").Location),
                MetadataReference.CreateFromFile(GetAssembly("System.Runtime").Location),
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(
                    Path.GetFileName(typeof(CollectionFluentAsserter<>).Assembly.Location)
                ),
                MetadataReference.CreateFromFile(Assembly.GetAssembly(typeof(Table))!.Location)
            )
            .AddSyntaxTrees(trees);

        using var ms = new MemoryStream();

        var result = compilation.Emit(ms);

        if (result.Success)
        {
            yield break;
        }

        var failures = result.Diagnostics
            .Where(diagnostic => diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error);

        foreach (var diagnostic in failures)
        {
            yield return diagnostic.GetMessage();
        }
    }
}