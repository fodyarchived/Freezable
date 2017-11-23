using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

public class TypeFinder
{
    ModuleDefinition moduleDefinition;
    IAssemblyResolver assemblyResolver;
    Action<string> logInfo;
    public MethodReference ExceptionConstructorReference;
    public TypeReference VolatileReference;

    public TypeFinder(ModuleDefinition moduleDefinition, IAssemblyResolver assemblyResolver, Action<string> logInfo)
    {
        this.moduleDefinition = moduleDefinition;
        this.assemblyResolver = assemblyResolver;
        this.logInfo = logInfo;
    }

    public void Execute()
    {
        var coreTypes = new List<TypeDefinition>();
        AddAssemblyIfExists("mscorlib", coreTypes);
        AddAssemblyIfExists("System.Runtime", coreTypes);
        AddAssemblyIfExists("netstandard", coreTypes);

        FindExceptionType(coreTypes);
        FindVolatileType(coreTypes);
    }

    void FindVolatileType(List<TypeDefinition> coreTypes)
    {
        var isVolatile = coreTypes.FirstOrDefault(x => x.Name == "IsVolatile");

        if (isVolatile == null)
        {
            throw new WeavingException("Could not find IsVolatile");
        }

        VolatileReference = moduleDefinition.ImportReference(isVolatile);
    }

    void FindExceptionType(List<TypeDefinition> coreTypes)
    {
        var exceptionType = coreTypes.FirstOrDefault(x => x.Name == "InvalidOperationException");

        if (exceptionType == null)
        {
            throw new WeavingException("Could not find InvalidOperationException");
        }

        var methodDefinition = exceptionType.Methods
            .First(x => x.IsConstructor &&
                        x.Parameters.Count == 1 &&
                        x.Parameters[0].ParameterType.Name == "String");

        ExceptionConstructorReference = moduleDefinition.ImportReference(methodDefinition);
    }

    void AddAssemblyIfExists(string name, List<TypeDefinition> types)
    {
        AssemblyDefinition msCoreLibDefinition;
        try
        {
            msCoreLibDefinition = assemblyResolver.Resolve(new AssemblyNameReference(name, null));
        }
        catch (AssemblyResolutionException)
        {
            logInfo($"Failed to resolve '{name}'. So skipping its types.");
            return;
        }
        if (msCoreLibDefinition == null)
        {
            return;
        }
        var module = msCoreLibDefinition.MainModule;
        types.AddRange(module.Types);
    }
}