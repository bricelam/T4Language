using System;
using Microsoft.CommonLanguageServerProtocol.Framework;

namespace T4Language.Server;

class LspLogger : ILspLogger
{
    public void LogEndContext(string message, params object[] @params)
    {
    }

    public void LogError(string message, params object[] @params)
    {
    }

    public void LogException(Exception exception, string message = null, params object[] @params)
    {
    }

    public void LogInformation(string message, params object[] @params)
    {
    }

    public void LogStartContext(string message, params object[] @params)
    {
    }

    public void LogWarning(string message, params object[] @params)
    {
    }
}
