using System;
using System.Collections.Generic;
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

            var sourceBuilder = new StringBuilder($@"using System.Threading;
using System.Threading.Tasks;

namespace {((NamespaceDeclarationSyntax) mediatorClass.Parent)?.Name}
{{
    public partial class {mediatorClass.Identifier}
    {{
");
            foreach (var handlerClass in GetRequestHandlers(context))
            {
                var types = LookupRequestAndResponseTypes(handlerClass);
                var fieldName = $"_{Char.ToLowerInvariant(handlerClass.Identifier.ValueText[0])}{handlerClass.Identifier.ValueText.Substring(1)}";
                
                sourceBuilder.AppendLine($"\t\tprivate readonly IRequestHandler<{types.RequestType}, {types.ResponseType}> {fieldName} = new {handlerClass.Identifier.ValueText}();");
                sourceBuilder.Append($"{handlerClass.Members[0].GetLeadingTrivia().ToString()}");
                sourceBuilder.AppendLine(
                    $@"public async Task<{types.ResponseType}> Send({types.RequestType} request, CancellationToken cancellationToken = default) 
        {{
            RequestHandlerDelegate<{types.ResponseType}> delegate0 = () => {fieldName}.Handle(request, cancellationToken);");

                int index = 0;
                foreach (var pipelineClass in GetPipelines(context).OrderByDescending(GetPipelineOrder))
                {
                    index++;
                    sourceBuilder.AppendLine($"\t\t\tRequestHandlerDelegate<{types.ResponseType}> delegate{index} = () => new {pipelineClass.Identifier}<{types.RequestType}, {types.ResponseType}>().Execute(request, cancellationToken, delegate{index - 1});");
                }

                sourceBuilder.AppendLine($@"            return await delegate{index}();
        }}");
                sourceBuilder.AppendLine(string.Empty);
            }

            sourceBuilder.AppendLine(@"    }
}
");
            
            
            context.AddSource("SourceMediator", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
        }

        private static int GetPipelineOrder(ClassDeclarationSyntax pipelineClass)
        {
            var pipelineOrderAttribute = pipelineClass.AttributeLists
                .SelectMany(al => al.Attributes)
                .SingleOrDefault(a => a.Name.ToString() == "PipelineOrder");

            int.TryParse(pipelineOrderAttribute?.ArgumentList?.Arguments[0].ToString() ?? "0", out var order);
            return order;
        }

        private static bool TryGetMediatorClass(GeneratorExecutionContext context, out ClassDeclarationSyntax mediatorClass)
        {
            MediatorSyntaxReceiver mediatorSyntaxReceiver = (MediatorSyntaxReceiver) context.SyntaxReceiver;
            mediatorClass = mediatorSyntaxReceiver?.MediatorClassDeclaration;
            return mediatorClass != null;
        }

        private static IEnumerable<ClassDeclarationSyntax> GetRequestHandlers(GeneratorExecutionContext context)
        {
            return ((MediatorSyntaxReceiver) context.SyntaxReceiver)?.MediatorRequestHandlers ?? Enumerable.Empty<ClassDeclarationSyntax>();
        }

        private static IEnumerable<ClassDeclarationSyntax> GetPipelines(GeneratorExecutionContext context)
        {
            return ((MediatorSyntaxReceiver) context.SyntaxReceiver)?.Pipelines ?? Enumerable.Empty<ClassDeclarationSyntax>();
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