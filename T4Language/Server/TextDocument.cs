using System.Collections.Generic;
using Mono.TextTemplating;

namespace T4Language.Server;

class TextDocument
{
    public ParsedTemplate ParsedTemplate { get; set; }
    public IReadOnlyList<string> Lines { get; set; }
    public IEnumerable<string> Words { get; set; }
}
