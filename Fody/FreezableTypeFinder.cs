using System.Linq;
using Mono.Cecil;

public class FreezableTypeFinder
{
    ModuleDefinition moduleDefinition;
    public TypeReference FreezableType;
    public MethodReference GetIsFrozenMethod;
    IAssemblyResolver assemblyResolver;

    public FreezableTypeFinder(ModuleDefinition moduleDefinition, IAssemblyResolver assemblyResolver)
    {
        this.moduleDefinition = moduleDefinition;
        this.assemblyResolver = assemblyResolver;
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
            var mainModule = assemblyResolver.Resolve(reference).MainModule;
            FreezableType = mainModule.Types.FirstOrDefault(x => x.Name == "IFreezable");
            if (FreezableType != null)
            {
                return;
            }
        }
    }
}