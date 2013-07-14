using System.Linq;
using Mono.Cecil;

public class VolatileTypeFinder
{
    ModuleDefinition moduleDefinition;
    IAssemblyResolver assemblyResolver;
    public TypeReference VolatileReference;

    public VolatileTypeFinder(ModuleDefinition moduleDefinition, IAssemblyResolver assemblyResolver)
    {
        this.moduleDefinition = moduleDefinition;
        this.assemblyResolver = assemblyResolver;
    }

    public void Execute()
    {
        var typeDefinition = GetTypeDefinition("mscorlib") ?? GetTypeDefinition("System.Runtime");
        VolatileReference = moduleDefinition.Import(typeDefinition);
    }

    TypeDefinition GetTypeDefinition(string assemblyName)
    {
        var msCoreLibDefinition = assemblyResolver.Resolve(assemblyName);
        return msCoreLibDefinition
            .MainModule
            .Types
            .FirstOrDefault(x => x.Name == "IsVolatile");
    }
}