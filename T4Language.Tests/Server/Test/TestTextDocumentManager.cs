using System;
using System.Collections.Generic;
using Mono.TextTemplating;

namespace T4Language.Server.Test;

class TestTextDocumentManager : TextDocumentManager
{
    readonly Dictionary<Uri, ParsedTemplate> _openDocuments = new Dictionary<Uri, ParsedTemplate>();

    public void Open(Uri uri, string content)
    {
        var template = new ParsedTemplate(uri.LocalPath);
        try
        {
            template.ParseWithoutIncludes(new Tokeniser(uri.LocalPath, content));
        }
        catch (ParserException ex)
        {
            template.LogError(ex.Message, ex.Location);
        }

        _openDocuments[uri] = template;
    }

    public override ParsedTemplate Get(Uri uri)
        => _openDocuments[uri];
}