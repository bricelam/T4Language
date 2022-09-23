using System;
using System.Collections.Generic;
using Mono.TextTemplating;

namespace T4Language.Server.Test;

class TestTextDocumentManager : TextDocumentManager
{
    readonly Dictionary<Uri, TextDocument> _openDocuments = new Dictionary<Uri, TextDocument>();

    public void Open(Uri uri, string content)
    {
        // TODO: DRY
        var parsedTemplate = new ParsedTemplate(uri.LocalPath);
        try
        {
            parsedTemplate.ParseWithoutIncludes(new Tokeniser(uri.LocalPath, content));
        }
        catch (ParserException ex)
        {
            parsedTemplate.LogError(ex.Message, ex.Location);
        }

        _openDocuments[uri] = new TextDocument
        {
            ParsedTemplate = parsedTemplate,
            Words = Array.Empty<string>()
        };
    }

    public override TextDocument Get(Uri uri)
        => _openDocuments[uri];
}