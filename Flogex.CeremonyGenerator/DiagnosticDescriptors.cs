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

        private static readonly Lazy<DiagnosticDescriptor> _typeMustBePartial = new Lazy<DiagnosticDescriptor>(() =>
            new DiagnosticDescriptor(
                "FG0001",
                $"Types marked with {nameof(GenerateEquatableAttribute)} must be partial",
                "{0} must be a partial {1} in order to implement IEquatable automatically",
                _errorCategory,
                DiagnosticSeverity.Error,
                true));

        private static readonly Lazy<DiagnosticDescriptor> _typeMustHavePublicProperties = new Lazy<DiagnosticDescriptor>(() =>
            new DiagnosticDescriptor(
                "FG0002",
                $"Types marked with {nameof(GenerateEquatableAttribute)} must have public properties",
                "{0} must have properties with a public getter in order to implement IEquatable automatically",
                _errorCategory,
                DiagnosticSeverity.Error,
                true));

        private static readonly Lazy<DiagnosticDescriptor> _typeMustBeClassOrStruct = new Lazy<DiagnosticDescriptor>(() =>
            new DiagnosticDescriptor(
                "FG0003",
                $"Only classes and structs can be marked with {nameof(GenerateEquatableAttribute)}",
                "{0} is not a class or struct. IEquatable can therefore not be implemented automatically",
                _errorCategory,
                DiagnosticSeverity.Error,
                true));

        /// <summary>
        /// The <see cref="Diagnosticr"/> to use when a class marked with
        /// <see cref="GenerateEquatableAttribute"/> is not partial.
        /// </summary>
        public static Diagnostic TypeMustBePartial(TypeDeclarationSyntax type)
        {
            return Diagnostic.Create(
                _typeMustBePartial.Value,
                type.GetLocation(),
                type.Identifier.ToString(),
                type is ClassDeclarationSyntax ? "class" : type is StructDeclarationSyntax ? "struct" : "type");
        }

        public static Diagnostic TypeMustHavePublicProperties(TypeDeclarationSyntax type)
        {
            return Diagnostic.Create(
                _typeMustHavePublicProperties.Value,
                type.GetLocation(),
                type.Identifier.ToString());
        }

        public static Diagnostic TypeMustBeClassOrStruct(TypeDeclarationSyntax type)
        {
            return Diagnostic.Create(
                _typeMustBeClassOrStruct.Value,
                type.GetLocation(),
                type.Identifier.ToString());
        }
    }
}
