using System.Collections.Generic;
using Mono.TextTemplating;

namespace T4Language.Server;

class TextDocument
{
    public ParsedTemplate ParsedTemplate { get; set; }
    public ICollection<string> Words { get; set; } = new HashSet<string>();
}
