using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.LanguageServer.Protocol;
using T4Language.Server.Test;
using Xunit;

namespace T4Language.Server.Handlers;

public class TextDocumentCompletionHandlerTests
{
    [Fact]
    public Task Directive_name()
        => Test(
            "<#@ t| #>",
            items => Assert.Contains(items, i => i.Label == "template"));

    [Fact]
    public Task Directive_attribute_name()
        => Test(
            "<#@ template d| #>",
            items => Assert.Contains(items, i => i.Label == "debug"));

    [Fact]
    public Task Directive_attribute_value()
        => Test(
            @"<#@ template debug=""t|"" #>",
            items => Assert.Contains(items, i => i.Label == "true"));

    [Fact]
    public Task Snippet()
        => Test(
            "i|",
            items => Assert.Contains(items, i => i.Label == "import"));

    async Task Test(string template, Action<CompletionItem[]> assert)
    {
        var uri = new Uri("file:///Test.tt");
        var textDocumentManager = new TextDocumentManager(new TestClientFacade());
        await textDocumentManager.OpenOrChangeAsync(
            uri,
            template.Replace("|", ""));
        var handler = new TextDocumentCompletionHandler(new SnippetsManager());
        var context = new RequestContext
        {
            TextDocument = textDocumentManager.Get(uri)
        };

        var result = await handler.HandleRequestAsync(
            new CompletionParams
            {
                TextDocument = new TextDocumentIdentifier
                {
                    Uri = uri
                },
                Position = TestHelper.GetPosition(template)
            },
            context);

        assert(result);
    }
}