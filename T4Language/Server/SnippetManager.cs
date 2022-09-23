using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.VisualStudio.TextMate.Snippets.Translator;

namespace T4Language.Server;

class SnippetsManager
{
    IEnumerable<SnippetInfo> _snippets;
    public IEnumerable<SnippetInfo> Snippets
        => _snippets ??= LoadSnippets();

    public IEnumerable<SnippetInfo> LoadSnippets()
    {
        var snippets = new List<SnippetInfo>();
        var errors = new List<SnippetConversionErrors>();
        var translated = SnippetTranslator.TryTranslateDirectoryOrFiles(
            // TODO: Look for a better way
            Path.Combine(Path.GetDirectoryName(typeof(SnippetsManager).Assembly.Location), "Snippets"),
            snippets,
            errors);
        Debug.Assert(translated);
        Debug.Assert(errors.Count == 0);

        return snippets;
    }
}
