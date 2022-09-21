using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.VisualStudio.LanguageServer.Protocol;
using Mono.TextTemplating;

namespace T4Language.Server;

class TextDocumentManager
{
    readonly LanguageServer _server;
    readonly Dictionary<Uri, ParsedTemplate> _openDocuments = new Dictionary<Uri, ParsedTemplate>();

    protected TextDocumentManager()
    {
    }

    public TextDocumentManager(LanguageServer server)
        => _server = server;

    public Task OpenOrChangeAsync(Uri uri, string content)
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

        return ReportDiagnosticsAsync(uri, template.Errors);
    }

    public virtual ParsedTemplate Get(Uri uri)
        => _openDocuments[uri];

    public void Close(Uri uri)
        => _openDocuments.Remove(uri);

    Task ReportDiagnosticsAsync(Uri uri, CompilerErrorCollection errors)
    {
        var diagnostics = new List<Diagnostic>(errors.Count);
        foreach (CompilerError error in errors)
        {
            if (error.FileName != uri.LocalPath)
            {
                Debug.Fail("Unexpected filename: " + error.FileName);

                continue;
            }

            diagnostics.Add(
                new Diagnostic
                {
                    Range = new()
                    {
                        Start = new Position(error.Line - 1, error.Column - 1),
                        End = new Position(error.Line - 1, error.Column - 1)
                    },
                    Severity = error.IsWarning
                        ? DiagnosticSeverity.Warning
                        : DiagnosticSeverity.Error,
                    Code = error.ErrorNumber,
                    Source = "Mono.TextTemplating",
                    Message = error.ErrorText
                });
        }

        return _server.ExecuteAsync(
            Methods.TextDocumentPublishDiagnostics,
            new PublishDiagnosticParams
            {
                Uri = uri,
                Diagnostics = diagnostics.ToArray()
            });
    }
}
