using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

public class ImplementsInterfaceFinder
{
    TypeResolver typeResolver;
    Dictionary<string, TypeDefinition> typeReferencesImplementingInterface;

    public ImplementsInterfaceFinder(TypeResolver typeResolver)
    {
        this.typeResolver = typeResolver;
        typeReferencesImplementingInterface = new Dictionary<string, TypeDefinition>();
    }

    public TypeDefinition HierachyImplementsIFreezable(TypeReference typeReference)
    {
        TypeDefinition baseType;
        var fullName = typeReference.FullName;
        if (!typeReferencesImplementingInterface.TryGetValue(fullName, out baseType))
        {
            var type = ToDefinition(typeReference);
            if (ImplementsInterface(type))
            {
                baseType = type;
            }
            else if (!ShouldSkipBaseType(type))
            {
                baseType = HierachyImplementsIFreezable(type.BaseType);
            }
        }

        return typeReferencesImplementingInterface[fullName] = baseType;
    }

    bool ShouldSkipBaseType(TypeDefinition type)
    {
        return type.BaseType == null || (type.BaseType.FullName.StartsWith("System.Collections"));
    }

    TypeDefinition ToDefinition(TypeReference typeReference)
    {
        if (typeReference.IsDefinition)
        {
            return (TypeDefinition) typeReference;
        }
        return typeResolver.Resolve(typeReference);
    }

    bool ImplementsInterface(TypeDefinition typeDefinition)
    {
        return typeDefinition.Interfaces.Any(x => x.Name == "IFreezable");
    }


}