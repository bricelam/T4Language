using System.Collections;
using System.Collections.Generic;

namespace T4Language.Server.Metadata;

class AttributeMetadata : IEnumerable
{
    public static readonly IEnumerable<AttributeMetadata> KnownAttributes = new AttributeMetadata[]
    {
        new("processor", "The directive processor to use.")
        {
            // TODO: Directive processors
        },
        new("requires", "A semicolon-separated list of name-value pairs. Use by directives that follow the requires-provides pattern."),
        new("provides", "A semicolon-separated list of name-value pairs. Use by directives that follow the requires-provides pattern.")
    };

    public AttributeMetadata(string name, string documentation)
    {
        Name = name;
        Documentation = documentation;
    }

    public string Name { get; }
    public string Documentation { get; }
    public ICollection<string> KnownValues { get; set; } = new List<string>();

    public void Add(string value)
        => KnownValues.Add(value);

    IEnumerator IEnumerable.GetEnumerator()
        => KnownValues.GetEnumerator();
}
