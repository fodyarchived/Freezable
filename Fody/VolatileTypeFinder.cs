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
        var msCoreLibDefinition = assemblyResolver.Resolve("mscorlib");

        var exceptionDefinition = msCoreLibDefinition
            .MainModule
            .Types
            .First(x => x.Name == "IsVolatile");
        VolatileReference = moduleDefinition.Import(exceptionDefinition);
    }

}