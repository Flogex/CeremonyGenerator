using System;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Xunit;
using Xunit.Sdk;

namespace Flogex.CeremonyGenerator.Tests
{
    public class SyntaxTreeComparisonTests
    {
        private static readonly string _workingDirectory = Environment.CurrentDirectory;

        [Fact]
        public void GeneratedSyntaxTreesShouldMatchOutputFiles()
        {
            var inputFiles = Directory.GetFiles(
                _workingDirectory,
                "*.Input.cs",
                SearchOption.AllDirectories);

            var exceptionAggregator = new ExceptionAggregator();

            foreach (var file in inputFiles)
                exceptionAggregator.Run(() => AssertSyntaxTreeIsAsExpected(file));

            if (exceptionAggregator.HasExceptions)
                throw exceptionAggregator.ToException();
        }

        private void AssertSyntaxTreeIsAsExpected(string file)
        {
            var actualSyntaxTree = GenerateExtendingSyntaxTree(file);

            var expectedSyntaxTree = GetExpectedSyntaxTree(file);

            var diff = expectedSyntaxTree.GetChanges(actualSyntaxTree);

            if (diff.Count != 0)
            {
                throw new SyntaxTreesNotEqualException(expectedSyntaxTree, actualSyntaxTree);
            }
        }

        private SyntaxTree GenerateExtendingSyntaxTree(string inputFile)
        {
            var extendeeFilePath = Path.Combine(_workingDirectory, inputFile);
            var extendeeSourceText = GetSourceTextFromFile(extendeeFilePath);
            var extendeeSyntaxTree = CSharpSyntaxTree.ParseText(extendeeSourceText);

            var receiver = new TypeDeclarationsReceiver();
            AddAttributes(receiver, extendeeSyntaxTree);

            var sourceGeneratorContext = SourceGeneratorContextExtensions.CreateInstance(receiver);

            var sut = new EquatableGenerator();
            sut.Execute(sourceGeneratorContext);

            var addedSources = sourceGeneratorContext.GetAddedSources();

            var generatedSourceText = addedSources
                .Select(_ => _.Text)
                .Single();

            return CSharpSyntaxTree.ParseText(generatedSourceText);
        }

        private static void AddAttributes(ISyntaxReceiver receiver, SyntaxTree syntaxTree)
        {
            var attributes = syntaxTree
                .GetRoot()
                .DescendantNodes()
                .Where(node => node.IsKind(SyntaxKind.Attribute));

            foreach (var attribute in attributes)
            {
                receiver.OnVisitSyntaxNode(attribute);
            }
        }

        private static SyntaxTree GetExpectedSyntaxTree(string inputFile)
        {
            var expectedOutputFile = GetExpectedOutputFile(inputFile);
            var expectedOutputSourceText = GetSourceTextFromFile(expectedOutputFile);
            return CSharpSyntaxTree.ParseText(expectedOutputSourceText);
        }

        private static string GetExpectedOutputFile(string inputFile)
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
    }
}
