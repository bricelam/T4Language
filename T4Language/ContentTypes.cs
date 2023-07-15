using System.ComponentModel.Composition;
using Microsoft.VisualStudio.LanguageServer.Client;
using Microsoft.VisualStudio.Utilities;

namespace T4Language;

static class ContentTypes
{
#pragma warning disable 649
    [Export]
    [Name("T4")]
    [BaseDefinition(CodeRemoteContentDefinition.CodeRemoteContentTypeName)]
    public static readonly ContentTypeDefinition T4ContentTypeDefinition;

    [Export]
    [FileExtension(".tt")]
    [ContentType("T4")]
    public static readonly FileExtensionToContentTypeDefinition TTFileExtensionDefinition;

    [Export]
    [FileExtension(".t4")]
    [ContentType("T4")]
    public static readonly FileExtensionToContentTypeDefinition T4FileExtensionDefinition;

    [Export]
    [FileExtension(".ttinclude")]
    [ContentType("T4")]
    public static readonly FileExtensionToContentTypeDefinition TTIncludeFileExtensionDefinition;
#pragma warning restore 649
}
