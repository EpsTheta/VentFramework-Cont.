using System;
using System.Collections.Generic;
using HarmonyLib;

namespace VentLib.Commands;

public class CommandContext
{
    public string OriginalMessage = null!;
    public string? Alias;
    public string[] Args = null!;
    public List<int> ErroredParameters = null!;

    internal PlayerControl Source = null!;
    
    internal CommandContext(PlayerControl source, string message)
    {
        OriginalMessage = message;
        string[] split = message.Split(" ");
        Alias = split[0];
        Args = split.Length > 1 ? split[1..] : Array.Empty<string>();
        ErroredParameters = new List<int>();
        Source = source;
    }

    private CommandContext()
    {
    }

    internal CommandContext Subcommand()
    {
        return new CommandContext
        {
            OriginalMessage = OriginalMessage,
            Alias = Args.Length > 0 ? Args[0] : null!,
            Args = Args.Length > 1  ? Args[1..] : Array.Empty<string>(),
            ErroredParameters = new List<int>(),
            Source = Source
        };
    }

    public string Join(string delimiter = " ") => Args.Join(delimiter: delimiter);
}