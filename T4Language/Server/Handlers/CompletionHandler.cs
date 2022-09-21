﻿using System;
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
        var template = _textDocumentManager.Get(request.TextDocument.Uri);
        var line = request.Position.Line + 1;
        var column = request.Position.Character + 1;

        var segment = template.RawSegments.FirstOrDefault(
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
                    && column >= item.Value.ValueLocation.Column
                    // TODO: Is whitespace allowed?
                    && column < item.Value.ValueLocation.Column + directive.Attributes[item.Key].Length + 3)
                {
                    var knownValues = knownAttributes
                        .Where(a => a.Name == item.Key)
                        .Select(a => a.KnownValues)
                        .FirstOrDefault()
                        ?? Array.Empty<string>();

                    // TODO: How do you trim the =" and " ?
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
                            Documentation = a.Documentation,

                        })
                    .ToArray());
        }

        return Task.FromResult<CompletionItem[]>(null);
    }
}
