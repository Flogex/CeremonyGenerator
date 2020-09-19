using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Xunit;

namespace Flogex.CeremonyGenerator.Tests
{
    public class SyntaxTreeComparisonTests
    {
        private static readonly string _workingDirectory
            = Environment.CurrentDirectory;
        private static readonly CSharpParseOptions _parseOptions
            = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Preview);

        [Theory]
        [ClassData(typeof(TestInputFiles))]
        public void GeneratedSyntaxTreeShouldMatchOutputFile(string inputFileName) =>
            AssertSyntaxTreeIsAsExpected(inputFileName);

        private static void AssertSyntaxTreeIsAsExpected(string fileName)
        {
            var generatedSyntaxTree = ExecuteSourceGenerator(fileName);

            var expectedSyntaxTree = GetExpectedSyntaxTree(fileName);

            var diff = expectedSyntaxTree.GetChanges(generatedSyntaxTree);

            if (diff.Count != 0)
            {
                throw new SyntaxTreesNotEqualException(expectedSyntaxTree, generatedSyntaxTree);
            }
        }

        private static SyntaxTree ExecuteSourceGenerator(string inputFile)
        {
            var extendeeFilePath = Path.Combine(_workingDirectory, inputFile);
            var extendeeSyntaxTree = CreateSyntaxTreeFromFile(extendeeFilePath);

            var compilation = CreateCompilation(extendeeSyntaxTree);

            var sut = new EquatableGenerator();
            var generatorDriver = CreateGeneratorDriver(sut).RunGenerators(compilation);

            var generatorRunResult = generatorDriver
                .GetRunResult()
                .Results
                .Single(_ => object.ReferenceEquals(_.Generator, sut));

            return generatorRunResult.GeneratedSources[0].SyntaxTree;
        }

        private static SyntaxTree CreateSyntaxTreeFromFile(string filePath)
        {
            var source = GetSourceTextFromFile(filePath);
            return CSharpSyntaxTree.ParseText(source);
        }

        private static CSharpCompilation CreateCompilation(SyntaxTree syntaxTree)
        {
            var compilationOptions = new CSharpCompilationOptions(
                OutputKind.DynamicallyLinkedLibrary,
                nullableContextOptions: NullableContextOptions.Enable);
            return CSharpCompilation.Create(
                "Flogex.CeremonyGenerator.Tests.Samples",
                syntaxTrees: new SyntaxTree[] { syntaxTree },
                options: compilationOptions);
        }

        private static GeneratorDriver CreateGeneratorDriver(ISourceGenerator generator) =>
            CSharpGeneratorDriver.Create(new ISourceGenerator[] { generator }, parseOptions: _parseOptions);

        private static SyntaxTree GetExpectedSyntaxTree(string inputFile)
        {
            var outputFilePath = GetExpectedOutputFilePath(inputFile);
            var expectedSourceText = GetSourceTextFromFile(outputFilePath);
            return CSharpSyntaxTree.ParseText(expectedSourceText, options: _parseOptions);
        }

        private static string GetExpectedOutputFilePath(string inputFile)
        {
            // inputFile matches pattern *.Input.cs
            var inputClassName = inputFile[0..^".Input.cs".Length];
            var expectedOutputFile = inputClassName + ".ExpectedOutput.cs";
            return Path.Combine(_workingDirectory, expectedOutputFile);
        }

        private static SourceText GetSourceTextFromFile(string filePath)
        {
            using var stream = File.OpenRead(filePath);
            return SourceText.From(stream, Encoding.UTF8);
        }

        private class TestInputFiles : IEnumerable<object[]>
        {
            private readonly IEnumerable<object[]> _testParameter;

            public TestInputFiles()
            {
                var inputFiles = Directory.GetFiles(
                    _workingDirectory,
                    "*.Input.cs",
                    SearchOption.AllDirectories);

                _testParameter = inputFiles.Select(file => new object[] { file });
            }

            public IEnumerator<object[]> GetEnumerator() => _testParameter.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
