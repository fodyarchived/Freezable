using System;
using System.Linq;
using Mono.Cecil;

public class FreezableTypeFinder
{
    ModuleDefinition moduleDefinition;
    Func<string, AssemblyDefinition> resolveAssembly;
    public TypeReference FreezableType;
    public MethodReference GetIsFrozenMethod;

    public FreezableTypeFinder(ModuleDefinition moduleDefinition, Func<string, AssemblyDefinition> resolveAssembly)
    {
        this.moduleDefinition = moduleDefinition;
        this.resolveAssembly = resolveAssembly;
    }

    public void Execute()
    {
        FreezableType = moduleDefinition.Types.FirstOrDefault(x => x.Name == "IFreezable");
        if (FreezableType != null)
        {
            return;
        }
        foreach (var reference in moduleDefinition.AssemblyReferences)
        {
            var mainModule = resolveAssembly(reference.Name).MainModule;
            FreezableType = mainModule.Types.FirstOrDefault(x => x.Name == "IFreezable");
            if (FreezableType != null)
            {
                return;
            }
        }
    }
}