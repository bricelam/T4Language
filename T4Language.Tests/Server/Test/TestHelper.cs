using System;
using System.IO;
using Microsoft.VisualStudio.LanguageServer.Protocol;

namespace T4Language.Server.Test;

static class TestHelper
{
    public static Position GetPosition(string template)
    {
        var lineNumber = 0;
        int character;

        var reader = new StringReader(template);
        string line;
        while ((line = reader.ReadLine()) is not null)
        {
            character = line.IndexOf("|");
            if (character != -1)
                return new Position(lineNumber, character);

            lineNumber++;
        }

        throw new Exception("No position marker!");
    }

}
