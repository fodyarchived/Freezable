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
        var exceptionType = GetExceptionType("mscorlib") ?? GetExceptionType("System.Runtime");

        var exceptionDefinition = exceptionType
            .Methods
            .First(x => x.IsConstructor && x.Parameters.Count == 1 && x.Parameters[0].ParameterType.Name == "String");
        ExceptionConstructorReference = moduleDefinition.ImportReference(exceptionDefinition);
    }

    TypeDefinition GetExceptionType(string assemblyName)
    {
        var msCoreLibDefinition = assemblyResolver.Resolve(new AssemblyNameReference(assemblyName, null));
        return msCoreLibDefinition
            .MainModule
            .Types
            .FirstOrDefault(x => x.Name == "InvalidOperationException");
    }
}