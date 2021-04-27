using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SourceMediator.Generator
{
    internal class MediatorSyntaxReceiver : ISyntaxReceiver
    {
        public ClassDeclarationSyntax MediatorClassDeclaration { get; private set; }

        public List<ClassDeclarationSyntax> MediatorRequestHandlers { get; } = new List<ClassDeclarationSyntax>();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is ClassDeclarationSyntax mediatorClassDeclaration
                && mediatorClassDeclaration.Identifier.ValueText == "Mediator")
            {
                MediatorClassDeclaration = mediatorClassDeclaration;
            }

            if (syntaxNode is ClassDeclarationSyntax classDeclarationSyntax
                && classDeclarationSyntax.BaseList != null)
            {
                foreach (var type in classDeclarationSyntax.BaseList.Types)
                {
                    if (type is SimpleBaseTypeSyntax baseType)
                    {
                        if (baseType.Type is GenericNameSyntax genericType)
                        {
                            if (genericType.Identifier.ValueText == "IRequestHandler"
                                && genericType.TypeArgumentList.Arguments.Count == 2)
                            {
                                MediatorRequestHandlers.Add(classDeclarationSyntax);
                            }
                        }
                    }
                }           
            }
        }
    }
}