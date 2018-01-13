using System;
using System.Collections.Generic;
using Mono.Cecil;

public class TypeResolver
{
    Dictionary<string, TypeDefinition> definitions;

    public TypeResolver()
    {
        definitions = new Dictionary<string, TypeDefinition>();
    }

    public TypeDefinition Resolve(TypeReference reference)
    {
        if (definitions.TryGetValue(reference.FullName, out var definition))
        {
            return definition;
        }
        return definitions[reference.FullName] = InnerResolve(reference);
    }

    static TypeDefinition InnerResolve(TypeReference reference)
    {
        try
        {
            return reference.Resolve();
        }
        catch (Exception exception)
        {
            throw new Exception($"Could not resolve '{reference.FullName}'.", exception);
        }
    }
}