using System.Collections.Generic;
using Mono.TextTemplating;

namespace T4Language.Server;

class TextDocument
{
    public ParsedTemplate ParsedTemplate { get; set; }

    public IReadOnlyList<string> Lines { get; set; }

    // TODO: Separate text blocks from control blocks?
    public IEnumerable<string> Words { get; set; } = new HashSet<string>();
}
