using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.LanguageServer.Protocol;
using T4Language.Server.Test;
using Xunit;

namespace T4Language.Server.Handlers;

public class TextDocumentDocumentHighlightHandlerTests
{
    [Fact]
    public Task Start_of_line()
        => Test(
            "|word not word",
            items => Assert.Collection(
                    items,
                    i =>
                    {
                        Assert.Equal(0, i.Range.Start.Line);
                        Assert.Equal(0, i.Range.Start.Character);
                        Assert.Equal(0, i.Range.End.Line);
                        Assert.Equal(4, i.Range.End.Character);
                    },
                    i =>
                    {
                        Assert.Equal(0, i.Range.Start.Line);
                        Assert.Equal(9, i.Range.Start.Character);
                        Assert.Equal(0, i.Range.End.Line);
                        Assert.Equal(13, i.Range.End.Character);
                    }));

    [Fact]
    public Task In_word()
        => Test(
            "wo|rd not word",
            items => Assert.Collection(
                    items,
                    i =>
                    {
                        Assert.Equal(0, i.Range.Start.Line);
                        Assert.Equal(0, i.Range.Start.Character);
                        Assert.Equal(0, i.Range.End.Line);
                        Assert.Equal(4, i.Range.End.Character);
                    },
                    i =>
                    {
                        Assert.Equal(0, i.Range.Start.Line);
                        Assert.Equal(9, i.Range.Start.Character);
                        Assert.Equal(0, i.Range.End.Line);
                        Assert.Equal(13, i.Range.End.Character);
                    }));

    [Fact(Skip = "Issue #25")]
    public Task End_of_word()
        => Test(
            "word| not word",
            items => Assert.Collection(
                    items,
                    i =>
                    {
                        Assert.Equal(0, i.Range.Start.Line);
                        Assert.Equal(0, i.Range.Start.Character);
                        Assert.Equal(0, i.Range.End.Line);
                        Assert.Equal(4, i.Range.End.Character);
                    },
                    i =>
                    {
                        Assert.Equal(0, i.Range.Start.Line);
                        Assert.Equal(9, i.Range.Start.Character);
                        Assert.Equal(0, i.Range.End.Line);
                        Assert.Equal(13, i.Range.End.Character);
                    }));

    [Fact]
    public Task Start_of_word()
        => Test(
            "word not |word",
            items => Assert.Collection(
                    items,
                    i =>
                    {
                        Assert.Equal(0, i.Range.Start.Line);
                        Assert.Equal(0, i.Range.Start.Character);
                        Assert.Equal(0, i.Range.End.Line);
                        Assert.Equal(4, i.Range.End.Character);
                    },
                    i =>
                    {
                        Assert.Equal(0, i.Range.Start.Line);
                        Assert.Equal(9, i.Range.Start.Character);
                        Assert.Equal(0, i.Range.End.Line);
                        Assert.Equal(13, i.Range.End.Character);
                    }));

    [Fact(Skip = "Issue #25")]
    public Task End_of_line()
        => Test(
            """
            word|
            not word
            """,
            items => Assert.Collection(
                    items,
                    i =>
                    {
                        Assert.Equal(0, i.Range.Start.Line);
                        Assert.Equal(0, i.Range.Start.Character);
                        Assert.Equal(0, i.Range.End.Line);
                        Assert.Equal(4, i.Range.End.Character);
                    },
                    i =>
                    {
                        Assert.Equal(1, i.Range.Start.Line);
                        Assert.Equal(4, i.Range.Start.Character);
                        Assert.Equal(1, i.Range.End.Line);
                        Assert.Equal(8, i.Range.End.Character);
                    }));

    [Fact]
    public Task End_of_file()
        => Test(
            """
            word not word
            |
            """,
            Assert.Null);

    async Task Test(string template, Action<DocumentHighlight[]> assert)
    {
        var uri = new Uri("file:///Test.tt");
        var textDocumentManager = new TextDocumentManager(new TestClientFacade());
        await textDocumentManager.OpenOrChangeAsync(
            uri,
            template.Replace("|", ""));

        var result = await new TextDocumentDocumentHighlightHandler().HandleRequestAsync(
            new DocumentHighlightParams
            {
                TextDocument = new TextDocumentIdentifier
                {
                    Uri = uri
                },
                Position = TestHelper.GetPosition(template)
            },
            new RequestContext
            {
                TextDocument = textDocumentManager.Get(uri)
            });

        assert(result);
    }
}