#nullable enable
using System;
using System.Linq;
using System.Reflection;
using VentLib.Logging;

namespace VentLib.RPC;

public static class VentRPCs
{
    // This is the sender version of this Rpc. In order to fully utilize it you must make your own handler.
    [ModRPC((uint)VentRPC.VersionCheck, RpcActors.Everyone, RpcActors.NonHosts)]
    public static void SendVersionPing(string fullAssemblyName, string? version = null, bool isCorrect = true)
    {
        AssemblyName name = new AssemblyName(fullAssemblyName);
        Assembly? assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(assembly => assembly.GetName().Name == name.Name);
        version = assembly?.GetName()?.Version?.ToString();
        bool correct = version == name.Version?.ToString();
        // ReSharper disable once TailRecursiveCall
        SendVersionPing(fullAssemblyName, version, correct);
    }
    
    [ModRPC((uint)VentRPC.SetControlFlag, RpcActors.Host, RpcActors.NonHosts)]
    public static void SetControlFlag(string assemblyName, int controlFlag)
    {
        VentLogger.Trace($"SetControlFlag(assemblyName={assemblyName}, controlFlag={controlFlag})", "VentFramework");
        Assembly? assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(assembly => assembly.GetName().FullName == assemblyName);
        if (assembly == null) return;
        VentFramework.SetControlFlag(assembly, (VentControlFlag)controlFlag);
        VentLogger.Trace($"Control Flag Set For: {assembly.GetName().Name} | Flag: {(VentControlFlag)controlFlag})");
    }
}

public enum VentRPC: uint
{
    VersionCheck = 1017,
    SetControlFlag = 1018
}