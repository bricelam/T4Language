using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CommonLanguageServerProtocol.Framework;
using Microsoft.VisualStudio.LanguageServer.Protocol;
using Mono.TextTemplating;
using T4Language.Server.Metadata;

namespace T4Language.Server.Handlers;

// TODO: DRY (TextDocumentCompletionHandler)
[LanguageServerEndpoint(Methods.TextDocumentHoverName)]
class TextDocumentHoverHandler : IRequestHandler<TextDocumentPositionParams, Hover, RequestContext>
{
    readonly TextDocumentManager _textDocumentManager;

    public TextDocumentHoverHandler(TextDocumentManager textDocumentManager)
        => _textDocumentManager = textDocumentManager;

    public bool MutatesSolutionState => false;

    public Task<Hover> HandleRequestAsync(
        TextDocumentPositionParams request,
        RequestContext context,
        CancellationToken cancellationToken)
    {
        var document = _textDocumentManager.Get(request.TextDocument.Uri);
        var line = request.Position.Line + 1;
        var column = request.Position.Character + 1;

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
                        .Where(d => string.Equals(d.Name, directive.Name, StringComparison.OrdinalIgnoreCase))
                        .Select(
                            d => new Hover
                            {
                                Contents = (SumType<string, MarkedString>)d.Documentation
                            })
                        .FirstOrDefault());
            }

            var knownAttributes = DirectiveMetadata.KnownDirectives
                .Where(d => string.Equals(d.Name, directive.Name, StringComparison.OrdinalIgnoreCase))
                .Select(d => d.KnownAttributes)
                .FirstOrDefault()
                ?? AttributeMetadata.KnownAttributes;

            foreach (var item in directive.AttributeInfo)
            {
                if (line == item.Value.NameLocation.Line
                    && column >= item.Value.NameLocation.Column
                    && column < item.Value.NameLocation.Column + item.Key.Length)
                {
                    return Task.FromResult(
                        knownAttributes
                            .Where(a => string.Equals(a.Name, item.Key, System.StringComparison.OrdinalIgnoreCase))
                            .Select(
                                a => new Hover
                                {
                                    Contents = (SumType<string, MarkedString>)a.Documentation
                                })
                            .FirstOrDefault());
                }
            }
        }

        return Task.FromResult<Hover>(null);
    }
}
