using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace T4Language.Server.Metadata;

class DirectiveMetadata : IEnumerable
{
    public static readonly IEnumerable<DirectiveMetadata> KnownDirectives = new DirectiveMetadata[]
    {
        new("template", "Specifies how the template should be transformed.")
        {
            new("compilerOptions", "The C# or VB compiler options to use."),
            new("culture", "The culture to use when converting to text.")
            {
                // TODO: Blank (Invariant)
                KnownValues = CultureInfo.GetCultures(CultureTypes.InstalledWin32Cultures)
                    .Select(c => c.Name)
                    .ToList()
            },
            new("debug", "Enables better debugging. Right-click on the template in Solution Explorer and choose Debug T4 Template.")
            {
                "true",
                "false"
            },
            new("hostspecific", "Adds the Host property for accessing to the host transforming the template.")
            {
                "true",
                "false",
                "trueFromBase"
            },
            new("language", "The language used inside control blocks.")
            {
                "C#",
                "VB"
            },
            new("inherits", "The base class to use when transforming the template. It should derive from TextTransformation.")
            {
                 // TODO: Types
            },
            new("linePragmas", "Removes line pragmas from the preprocessed class.")
            {
                "true",
                "false"
            },
            new("visibility", "The visibility to use for the preprocessed class.")
            {
                "public",
                "internal"
            }
        },
        new("parameter", "Add a property for accessing an argument passed into the template.")
        {
            new("type", "The fully-qualified type.")
            {
                // TODO: Types
            },
            new("name", "The name.")
        },
        new("output", "Defines the output file extension and encoding.")
        {
            new("extension", "The output file extension."),
            new("encoding", "The output file encoding.")
            {
                // TODO: 0 (System default)
                KnownValues = Encoding.GetEncodings()
                    .Select(e => e.Name)
                    .ToList()
            }
        },
        new("assembly", "Adds an assembly reference.")
        {
            new("name", "The assembly name or path.")
            {
                // TODO: Assemblies, Expand properties
            }
        },
        new("import", "Imports (adds a using for) a namespace to use inside control blocks.")
        {
            new("namespace", "The namespace.")
            {
                // TODO: Namespaces
            }
        },
        new("include", "Adds text from another file to the template.")
        {
            new("file", "The file path.")
            {
                // TODO: Files, Expand properties
            },
            new("once", "Only ever include the file once.")
            {
                "true"
            }
        }
    };

    public DirectiveMetadata(string name, string documentation)
    {
        Name = name;
        Documentation = documentation;
    }

    public string Name { get; }
    public string Documentation { get; }
    public ICollection<AttributeMetadata> KnownAttributes { get; } = new List<AttributeMetadata>();

    public void Add(AttributeMetadata attribute)
        => KnownAttributes.Add(attribute);

    IEnumerator IEnumerable.GetEnumerator()
        => KnownAttributes.GetEnumerator();
}
