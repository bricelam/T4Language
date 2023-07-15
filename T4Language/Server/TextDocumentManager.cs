using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
      '/'
    };

    readonly IClientFacade _clientFacade;
    readonly Dictionary<Uri, TextDocument> _openDocuments = new Dictionary<Uri, TextDocument>();

    public TextDocumentManager(IClientFacade clientFacade)
        => _clientFacade = clientFacade;

    public async Task OpenOrChangeAsync(Uri uri, string content)
    {
        var parsedTemplate = new ParsedTemplate(uri.LocalPath);
        try
        {
            parsedTemplate.ParseWithoutIncludes(new Tokeniser(uri.LocalPath, content));
        }
        catch (ParserException ex)
        {
            parsedTemplate.LogError(ex.Message, ex.Location);
        }

        await ReportDiagnosticsAsync(uri, parsedTemplate.Errors);

        var lines = new List<string>();
        var words = new HashSet<string>();

        string line;
        using var reader = new StringReader(content);
        while ((line = reader.ReadLine()) is not null)
        {
            lines.Add(line);

            foreach (var word in line.Split(_wordDelimiters).Where(w => w.Length > 1))
            {
                words.Add(word);
            }
        }

        _openDocuments[uri] = new TextDocument
        {
            ParsedTemplate = parsedTemplate,
            Lines = lines,
            Words = words
        };
    }

    public TextDocument Get(Uri uri)
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

        return _clientFacade.ExecuteAsync(
            Methods.TextDocumentPublishDiagnostics,
            new PublishDiagnosticParams
            {
                Uri = uri,
                Diagnostics = diagnostics.ToArray()
            });
    }
}
