using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.LanguageServer.Protocol;
using Mono.TextTemplating;

namespace T4Language.Server;

class TextDocumentManager
{
    static readonly char[] _wordDelimiters = new[]
    {
      ' ',
      '\t',
      '.',
      ',',
      '(',
      ')',
      '{',
      '}',
      '"',
      '\'',
      ':',
      '[',
      ']',
      ';',
      '+',
      '-',
      '*',
      '/',
      '\r',
      '\n'
    };

    readonly LanguageServer _server;
    readonly Dictionary<Uri, TextDocument> _openDocuments = new Dictionary<Uri, TextDocument>();

    protected TextDocumentManager()
    {
    }

    public TextDocumentManager(LanguageServer server)
        => _server = server;

    public async Task OpenOrChangeAsync(Uri uri, string content)
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

        await ReportDiagnosticsAsync(uri, template.Errors);

        _openDocuments[uri] = new TextDocument
        {
            ParsedTemplate = template,

            // TODO: Separate text blocks from control blocks?
            Words = new HashSet<string>(
                content.Split(_wordDelimiters, StringSplitOptions.RemoveEmptyEntries)
                    .Where(w => w.Length > 1))
        };
    }

    public virtual TextDocument Get(Uri uri)
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
