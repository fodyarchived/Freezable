using System.Linq;
using Mono.Cecil;


public class ExceptionFinder
{
    ModuleDefinition moduleDefinition;
    IAssemblyResolver assemblyResolver;
    public MethodReference ExceptionConstructorReference;

    public ExceptionFinder(ModuleDefinition moduleDefinition, IAssemblyResolver assemblyResolver)
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
            .First(x => x.Name == "InvalidOperationException")
            .Methods
            .First(x => x.IsConstructor && x.Parameters.Count == 1 && x.Parameters[0].ParameterType.Name=="String");
        ExceptionConstructorReference = moduleDefinition.Import(exceptionDefinition);

    }


}