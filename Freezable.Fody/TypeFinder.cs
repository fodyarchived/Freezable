using System;
using System.Linq;
using Mono.Cecil;

public class TypeFinder
{
    ModuleDefinition moduleDefinition;
    Func<string, TypeDefinition> findType;
    public MethodReference ExceptionConstructorReference;
    public TypeReference VolatileReference;

    public TypeFinder(ModuleDefinition moduleDefinition, Func<string, TypeDefinition> findType)
    {
        this.moduleDefinition = moduleDefinition;
        this.findType = findType;
    }

    public void Execute()
    {
        FindExceptionType();
        FindVolatileType();
    }

    void FindVolatileType()
    {
        var isVolatile = findType("System.Runtime.CompilerServices.IsVolatile");

        VolatileReference = moduleDefinition.ImportReference(isVolatile);
    }

    void FindExceptionType()
    {
        var exceptionType = findType("System.InvalidOperationException");

        var methodDefinition = exceptionType.Methods
            .First(x => x.IsConstructor &&
                        x.Parameters.Count == 1 &&
                        x.Parameters[0].ParameterType.Name == "String");

        ExceptionConstructorReference = moduleDefinition.ImportReference(methodDefinition);
    }
}