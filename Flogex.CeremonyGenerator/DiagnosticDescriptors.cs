using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Flogex.CeremonyGenerator
{
    /// <summary>
    /// Contains <see cref="Diagnostic"/> that is used for common errors.
    /// </summary>
    internal static class DiagnosticDescriptors
    {
        private const string _errorCategory = "Flogex.CeremonyGenerator.Errors";

        private static readonly Lazy<DiagnosticDescriptor> _classMustBePartial = new Lazy<DiagnosticDescriptor>(() =>
            new DiagnosticDescriptor(
                "FG0001",
                $"Classes marked with {nameof(GenerateEquatableAttribute)} must be partial",
                "{0} must be a partial class in order to implement IEquatable automatically",
                _errorCategory,
                DiagnosticSeverity.Error,
                true));

        private static readonly Lazy<DiagnosticDescriptor> _classMustHavePublicProperties = new Lazy<DiagnosticDescriptor>(() =>
            new DiagnosticDescriptor(
                "FG0002",
                $"Classes marked with {nameof(GenerateEquatableAttribute)} must have properties with public getter",
                "{0} must have properties with a public getter in order to implement IEquatable automatically",
                _errorCategory,
                DiagnosticSeverity.Error,
                true));

        /// <summary>
        /// The <see cref="Diagnosticr"/> to use when a class marked with
        /// <see cref="GenerateEquatableAttribute"/> is not partial.
        /// </summary>
        public static Diagnostic ClassMustBePartial(ClassDeclarationSyntax @class)
        {
            return Diagnostic.Create(
                _classMustBePartial.Value,
                @class.GetLocation(),
                @class.Identifier.ToString());
        }

        public static Diagnostic ClassMustHavePublicProperties(ClassDeclarationSyntax @class)
        {
            return Diagnostic.Create(
                _classMustHavePublicProperties.Value,
                @class.GetLocation(),
                @class.Identifier.ToString());
        }
    }
}
