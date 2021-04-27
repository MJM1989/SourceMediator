using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace SourceMediator.Generator
{
    [Generator]
    public class MediatorGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new MediatorSyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (!TryGetMediatorClass(context, out var mediatorClass))
                return;

            var sourceBuilder = new StringBuilder($@"
using System.Threading;
using System.Threading.Tasks;

namespace {((NamespaceDeclarationSyntax) mediatorClass.Parent)?.Name}
{{
    public partial class {mediatorClass.Identifier}
    {{
");
            foreach (var handlerClass in ((MediatorSyntaxReceiver) context.SyntaxReceiver)?.MediatorRequestHandlers ?? Enumerable.Empty<ClassDeclarationSyntax>())
            {
                var types = LookupRequestAndResponseTypes(handlerClass);
                var fieldName = $"_{Char.ToLowerInvariant(handlerClass.Identifier.ValueText[0])}{handlerClass.Identifier.ValueText.Substring(1)}";

                sourceBuilder.AppendLine($"\t\tprivate readonly IRequestHandler<{types.RequestType}, {types.ResponseType}> {fieldName} = new {handlerClass.Identifier.ValueText}();");
                sourceBuilder.AppendLine($@"
        public async Task<{types.ResponseType}> Send({types.RequestType} request, CancellationToken cancellationToken = default)
        {{
            return await {fieldName}.Handle(request, cancellationToken);
        }}");
                sourceBuilder.AppendLine("");
            }

            sourceBuilder.AppendLine(@"
    }}
}}
");
            context.AddSource("SourceMediator", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
        }

        private static bool TryGetMediatorClass(GeneratorExecutionContext context, out ClassDeclarationSyntax mediatorClass)
        {
            var mediatorSyntaxReceiver = (MediatorSyntaxReceiver) context.SyntaxReceiver;
            mediatorClass = mediatorSyntaxReceiver?.MediatorClassDeclaration;
            return mediatorClass != null;
        }

        private (string RequestType, string ResponseType) LookupRequestAndResponseTypes(TypeDeclarationSyntax requestHandler)
        {
            foreach (var entry in requestHandler.BaseList.Types)
            {
                if (entry is SimpleBaseTypeSyntax basetype)
                {
                    if (basetype.Type is GenericNameSyntax type)
                    {
                        if (type.Identifier.ValueText == "IRequestHandler"
                             && type.TypeArgumentList.Arguments.Count == 2)
                        {
                            return (type.TypeArgumentList.Arguments[0].ToString(),
                                type.TypeArgumentList.Arguments[1].ToString());
                        }
                    }
                }
            }
            return ("","");
        }

    }
}