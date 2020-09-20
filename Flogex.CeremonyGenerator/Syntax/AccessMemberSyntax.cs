using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Flogex.CeremonyGenerator.Syntax
{
    internal static class AccessMemberSyntax
    {
        public static MemberAccessExpressionSyntax Create(string call)
        {
            var arr = call.Split('.');
            var calledObj = arr[0];
            var calledMember = arr[1];

            return MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                IdentifierName(calledObj),
                IdentifierName(calledMember));
        }
    }
}
