using System.Text;
using Flogex.CeremonyGenerator.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Flogex.CeremonyGenerator
{
    /// <summary>
    /// Generates the IEquatable implementation for classes or structs
    /// marked with <see cref="GenerateEquatableAttribute"/>.
    /// </summary>
    [Generator]
    public class EquatableGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
            => context.RegisterForSyntaxNotifications(() => new TypeDeclarationsReceiver());

        public void Execute(GeneratorExecutionContext context)
        {
            var receiver = (TypeDeclarationsReceiver)context.SyntaxReceiver!;

            foreach (var extendee in receiver.ReceivedClasses)
            {
                TypeDeclarationSyntax? partialType = EquatableClassSyntax.Create(extendee, out var diagnostic);

                if (partialType is null)
                {
                    context.ReportDiagnostic(diagnostic!); //TODO Result class
                    continue;
                }

                var @namespace = extendee.GetContainingNamespace();

                MemberDeclarationSyntax compilationUnitMember = @namespace != null
                    ? NamespaceDeclaration(@namespace.Name).WithSingleMember(partialType)
                    : partialType;

                var compilationUnit = CompilationUnit()
                    .WithSingleUsing(UsingDirective(IdentifierName("System"))) // Needed for IEquatable and HashCode
                    .WithSingleMember(compilationUnitMember);

                var source = SourceText.From(compilationUnit.NormalizeWhitespace().ToFullString(), Encoding.UTF8);
                context.AddSource(extendee.Identifier.ToString() + ".WithEquatable.generated", source);
            }

            //TODO Process structs
        }
    }
}