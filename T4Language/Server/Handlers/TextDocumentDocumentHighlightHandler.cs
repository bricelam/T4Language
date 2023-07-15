using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CommonLanguageServerProtocol.Framework;
using Microsoft.VisualStudio.LanguageServer.Protocol;

namespace T4Language.Server.Handlers;

[LanguageServerEndpoint(Methods.TextDocumentDocumentHighlightName)]
class TextDocumentDocumentHighlightHandler : IRequestHandler<DocumentHighlightParams, DocumentHighlight[], RequestContext>
{
    public bool MutatesSolutionState => false;

    public Task<DocumentHighlight[]> HandleRequestAsync(
        DocumentHighlightParams request,
        RequestContext context,
        CancellationToken cancellationToken = default)
    {
        if (request.Position.Line >= context.TextDocument.Lines.Count)
            return Task.FromResult<DocumentHighlight[]>(null);

        var wordLine = context.TextDocument.Lines[request.Position.Line];
        if (request.Position.Character >= wordLine.Length
            || !IsIdentifierChar(wordLine[request.Position.Character]))
            return Task.FromResult<DocumentHighlight[]>(null);

        var highlights = new List<DocumentHighlight>();

        var wordStart = request.Position.Character;
        while (wordStart - 1 >= 0
            && IsIdentifierChar(wordLine[wordStart - 1]))
        {
            wordStart--;
        }

        var wordEnd = request.Position.Character + 1;
        while (wordEnd < wordLine.Length
            && IsIdentifierChar(wordLine[wordEnd]))
        {
            wordEnd++;
        }

        var word = wordLine.Substring(wordStart, wordEnd - wordStart);

        for (var line = 0; line < context.TextDocument.Lines.Count; line++)
        {
            var currentLine = context.TextDocument.Lines[line];
            var currentIndex = 0;
            while (currentIndex < currentLine.Length)
            {
                var foundAt = currentLine.IndexOf(word, currentIndex);
                if (foundAt == -1)
                    break;

                highlights.Add(
                    new DocumentHighlight
                    {
                        Range = new Range
                        {
                            Start = new Position(line, foundAt),
                            End = new Position(line, foundAt + word.Length)
                        }
                    });

                currentIndex = foundAt + word.Length;
            }
        }

        return Task.FromResult(highlights.ToArray());
    }

    static bool IsIdentifierChar(char c)
        => c == '_' || c == '$' || char.IsLetterOrDigit(c);
}
