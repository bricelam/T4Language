using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CommonLanguageServerProtocol.Framework;
using Microsoft.VisualStudio.LanguageServer.Protocol;
using Mono.TextTemplating;
using T4Language.Server.Metadata;

namespace T4Language.Server.Handlers;

[LanguageServerEndpoint(Methods.TextDocumentCompletionName)]
class CompletionHandler : IRequestHandler<CompletionParams, CompletionItem[], RequestContext>
{
    readonly TextDocumentManager _textDocumentManager;

    public CompletionHandler(TextDocumentManager textDocumentManager)
        => _textDocumentManager = textDocumentManager;

    public bool MutatesSolutionState => false;

    public Task<CompletionItem[]> HandleRequestAsync(
        CompletionParams request,
        RequestContext context = null,
        CancellationToken cancellationToken = default)
    {
        var document = _textDocumentManager.Get(request.TextDocument.Uri);
        var line = request.Position.Line + 1;
        var column = request.Position.Character + 1;

        var segment = document.ParsedTemplate.RawSegments.FirstOrDefault(
            s => line >= s.StartLocation.Line
                && line <= s.EndLocation.Line
                && column >= s.StartLocation.Column
                && column < s.EndLocation.Column);
        if (segment is Directive directive)
        {
            // TODO: Handle empty directives
            if (line == directive.StartLocation.Line
                && column <= directive.StartLocation.Column + directive.Name.Length)
            {
                // TODO: Why to these replace the end delimiter?
                return Task.FromResult(
                    DirectiveMetadata.KnownDirectives
                        .Select(
                            d => new CompletionItem
                            {
                                Kind = CompletionItemKind.Element,
                                Label = d.Name,
                                Documentation = d.Documentation
                            })
                        .ToArray());
            }

            var knownAttributes = DirectiveMetadata.KnownDirectives
                .Where(d => d.Name == directive.Name)
                .Select(d => d.KnownAttributes)
                .FirstOrDefault()
                ?? AttributeMetadata.KnownAttributes;

            foreach (var item in directive.AttributeInfo)
            {
                if (line == item.Value.ValueLocation.Line
                    // TODO: Move the `+ 2` logic into the parser
                    && column >= item.Value.ValueLocation.Column + 2
                    && column < item.Value.ValueLocation.Column + 2 + directive.Attributes[item.Key].Length)
                {
                    var knownValues = knownAttributes
                        .Where(a => a.Name == item.Key)
                        .Select(a => a.KnownValues)
                        .FirstOrDefault()
                        ?? Array.Empty<string>();

                    // TODO: Why do these replace the attribute name?
                    return Task.FromResult(
                        knownValues
                            .Select(
                                v => new CompletionItem
                                {
                                    Kind = CompletionItemKind.Value,
                                    Label = v
                                })
                            .ToArray());
                }
            }

            return Task.FromResult(
                knownAttributes
                    .Select(
                        a => new CompletionItem
                        {
                            Kind = CompletionItemKind.Property,
                            Label = a.Name,
                            InsertText = a.Name + @"=""$0""",
                            InsertTextFormat = InsertTextFormat.Snippet,
                            Documentation = a.Documentation
                        })
                    .ToArray());
        }

        return Task.FromResult(document.Words.Select(w => new CompletionItem { Label = w }).ToArray());
    }
}
