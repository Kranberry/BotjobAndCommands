using Microsoft.CodeAnalysis.CSharp;

namespace BotJobAndCommands.Scripting;

public class CsharpScripting : IScriptLoader
{
    public bool LoadScript(string script)
    {
        throw new NotImplementedException();
    }

    public bool LoadScripts(string? path)
    {
        throw new NotImplementedException();
    }

    #region Available Methods to scripting
    public static Action<string> Print = ScriptingInterface.Print;
    #endregion
}