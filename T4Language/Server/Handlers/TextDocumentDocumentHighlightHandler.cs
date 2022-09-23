using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CommonLanguageServerProtocol.Framework;
using Microsoft.VisualStudio.LanguageServer.Protocol;

namespace T4Language.Server.Handlers;

[LanguageServerEndpoint(Methods.TextDocumentDocumentHighlightName)]
class TextDocumentDocumentHighlightHandler : IRequestHandler<DocumentHighlightParams, DocumentHighlight[], RequestContext>
{
    readonly TextDocumentManager _textDocumentManager;

    public TextDocumentDocumentHighlightHandler(TextDocumentManager textDocumentManager)
        => _textDocumentManager = textDocumentManager;

    public bool MutatesSolutionState => false;

    public Task<DocumentHighlight[]> HandleRequestAsync(
        DocumentHighlightParams request,
        RequestContext context,
        CancellationToken cancellationToken)
    {
        var document = _textDocumentManager.Get(request.TextDocument.Uri);

        var wordLine = document.Lines[request.Position.Line];
        if (!IsIdentifierChar(wordLine[request.Position.Character]))
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
        highlights.Add(
            new DocumentHighlight
            {
                Range = new Range
                {
                    Start = new Position(request.Position.Line, wordStart),
                    End = new Position(request.Position.Line, wordEnd)
                }
            });

        for (var line = 0; line < document.Lines.Count; line++)
        {
            var currentLine = document.Lines[line];
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
