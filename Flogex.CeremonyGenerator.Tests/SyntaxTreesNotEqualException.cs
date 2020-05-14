using Microsoft.CodeAnalysis;
using Xunit.Sdk;

namespace Flogex.CeremonyGenerator.Tests
{
    public class SyntaxTreesNotEqualException : AssertActualExpectedException
    {
        private const string _message = "Generated SyntaxTree differs from the expected one.";

        public SyntaxTreesNotEqualException(
            SyntaxTree expected,
            SyntaxTree actual)
            : base(expected, actual, _message, "Expected SyntaxTree", "Actual SyntaxTree") { }
    }
}
