using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Flogex.CeremonyGenerator.Tests
{
    internal static class Helper
    {
        public static CSharpCompilation CreateCompilation(this SyntaxTree syntaxTree)
        {
            var compilationOptions = new CSharpCompilationOptions(
                OutputKind.DynamicallyLinkedLibrary,
                nullableContextOptions: NullableContextOptions.Enable);
            return CSharpCompilation.Create(
                "Flogex.CeremonyGenerator.Tests.Samples",
                syntaxTrees: new SyntaxTree[] { syntaxTree },
                options: compilationOptions);
        }

        public static GeneratorRunResult RunGenerator(this ISourceGenerator generator, Compilation compilation)
        {
            var generatorDriver = CSharpGeneratorDriver.Create(new ISourceGenerator[] { generator }) as GeneratorDriver;
            generatorDriver = generatorDriver.RunGenerators(compilation);

            return generatorDriver
                .GetRunResult()
                .Results
                .Single(_ => object.ReferenceEquals(_.Generator, generator));
        }
    }
}
