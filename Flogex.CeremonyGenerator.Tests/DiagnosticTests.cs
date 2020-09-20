using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace Flogex.CeremonyGenerator.Tests
{
    public class DiagnosticTests
    {
        private static GeneratorRunResult RunGeneratorOnSource(string source)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(source);
            var compilation = syntaxTree.CreateCompilation();

            var sut = new EquatableGenerator();
            return sut.RunGenerator(compilation);
        }

        [Fact]
        public void ClassIsNotPartial()
        {
            const string nonPartialClass = "[GenerateEquatable] class X { public int Prop { get; set; } }";
            var generatorRunResult = RunGeneratorOnSource(nonPartialClass);
            generatorRunResult.Diagnostics.Should().HaveCount(1)
                .And.SatisfyRespectively(_ => _.GetMessage().Should().Contain("partial class"));
        }

        [Fact]
        public void ClassHasNoPublicProperties()
        {
            const string classWithoutProperties = "[GenerateEquatable] partial class X { }";
            var generatorRunResult = RunGeneratorOnSource(classWithoutProperties);
            generatorRunResult.Diagnostics.Should().HaveCount(1)
                .And.SatisfyRespectively(_ => _.GetMessage().Should().Match("*properties * public getter*"));
        }

        [Fact]
        public void InterfaceHasEquatableAttribute()
        {
            const string decoratedInterface = "[GenerateEquatable] interface X { }";
            var generatorRunResult = RunGeneratorOnSource(decoratedInterface);
            generatorRunResult.Diagnostics.Should().HaveCount(1)
                .And.SatisfyRespectively(_ => _.GetMessage().Should().Match("*class or struct*"));
        }
    }
}
