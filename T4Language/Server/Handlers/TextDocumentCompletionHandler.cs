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
class TextDocumentCompletionHandler : IRequestHandler<CompletionParams, CompletionItem[], RequestContext>
{
    readonly TextDocumentManager _textDocumentManager;
    readonly SnippetsManager _snippetsManager;

    public TextDocumentCompletionHandler(TextDocumentManager textDocumentManager, SnippetsManager snippetsManager)
    {
        _textDocumentManager = textDocumentManager;
        _snippetsManager = snippetsManager;
    }

    public bool MutatesSolutionState => false;

    public Task<CompletionItem[]> HandleRequestAsync(
        CompletionParams request,
        RequestContext context = null,
        CancellationToken cancellationToken = default)
    {
        var document = _textDocumentManager.Get(request.TextDocument.Uri);
        var line = request.Position.Line + 1;
        var column = request.Position.Character + 1;

        // TODO: Can this be simplified?
        var segment = document.ParsedTemplate.RawSegments.FirstOrDefault(
            s => (line > s.StartLocation.Line && line < s.EndLocation.Line)
                || ((line == s.StartLocation.Line && column >= s.StartLocation.Column)
                    && ((s.StartLocation.Line != s.EndLocation.Line) || column < s.EndLocation.Column)
                || (line == s.EndLocation.Line && column < s.EndLocation.Column)));
        if (segment is Directive directive)
        {
            if (line == directive.StartLocation.Line
                && column <= directive.StartLocation.Column + directive.Name.Length)
            {
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
                    // TODO: Handle whitespace (in the Parser)
                    && column >= item.Value.ValueLocation.Column + 2
                    && column < item.Value.ValueLocation.Column + 2 + directive.Attributes[item.Key].Length)
                {
                    var knownValues = knownAttributes
                        .Where(a => a.Name == item.Key)
                        .Select(a => a.KnownValues)
                        .FirstOrDefault()
                        ?? Array.Empty<string>();

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
        else if (segment is TemplateSegment templateSegment
            && templateSegment.Type == SegmentType.Content)
        {
            return Task.FromResult(
                Enumerable.Concat(
                    _snippetsManager.Snippets
                        .Select(
                            s => new CompletionItem
                            {
                                Kind = CompletionItemKind.Snippet,
                                Label = s.Shortcut,
                                Documentation = s.Description,
                                InsertText = s.OriginalSnippetCode,
                                InsertTextFormat = InsertTextFormat.Snippet
                            }),
                    document.Words
                        .Where(w => !_snippetsManager.Snippets.Any(s => s.Shortcut == w))
                        .Select(w => new CompletionItem { Label = w }))
                    .ToArray());
        }

        return Task.FromResult(document.Words.Select(w => new CompletionItem { Label = w }).ToArray());
    }
}
