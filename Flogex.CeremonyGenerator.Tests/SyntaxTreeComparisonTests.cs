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
            var compilation = extendeeSyntaxTree.CreateCompilation();

            var sut = new EquatableGenerator();
            var generatorRunResult = sut.RunGenerator(compilation);

            return generatorRunResult.GeneratedSources[0].SyntaxTree;
        }

        private static SyntaxTree CreateSyntaxTreeFromFile(string filePath)
        {
            var source = GetSourceTextFromFile(filePath);
            return CSharpSyntaxTree.ParseText(source);
        }

        private static SyntaxTree GetExpectedSyntaxTree(string inputFile)
        {
            var outputFilePath = GetExpectedOutputFilePath(inputFile);
            var expectedSourceText = GetSourceTextFromFile(outputFilePath);
            return CSharpSyntaxTree.ParseText(expectedSourceText);
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
