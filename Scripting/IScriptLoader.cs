namespace BotJobAndCommands.Scripting;

public interface IScriptLoader
{
    /// <summary>
    /// Load script by name
    /// </summary>
    /// <param name="script">The name of the script to load</param>
    /// <returns>True if script was loaded succesfully</returns>
    public bool LoadScript(string script);

    /// <summary>
    /// Load scripts in selected folder
    /// </summary>
    /// <param name="path">The path to the folder, if null then every script loads</param>
    /// <returns>True if scripts was loaded successfully</returns>
    public bool LoadScripts(string? path);
}