using System.Linq;
using Fody;
using Mono.Cecil;

public class FreezeCheckerInjector
{
    ModuleDefinition moduleDefinition;
    FieldInjector fieldInjector;
    CheckIsFrozenBuilder checkIsFrozenBuilder;
    public MethodDefinition CheckIsFrozenMethod;

    public FreezeCheckerInjector(ModuleDefinition moduleDefinition, FieldInjector fieldInjector, CheckIsFrozenBuilder checkIsFrozenBuilder)
    {
        this.moduleDefinition = moduleDefinition;
        this.fieldInjector = fieldInjector;
        this.checkIsFrozenBuilder = checkIsFrozenBuilder;
    }

    public MethodReference Execute(TypeDefinition type)
    {
        CheckIsFrozenMethod = type.Methods.FirstOrDefault(IsCheckMethod);
        if (CheckIsFrozenMethod != null)
        {
            if (CheckIsFrozenMethod.IsStatic)
            {
                throw new WeavingException("CheckIfFrozen method can no be static");
            }
            if (!CheckIsFrozenMethod.IsFamily)
            {
                throw new WeavingException("CheckIfFrozen method needs to be protected");
            }
            return moduleDefinition.ImportReference(CheckIsFrozenMethod);
        }
        if (type.Module != moduleDefinition)
        {
            throw new WeavingException($"Could not inject to '{type.FullName}' because it is not in the target assembly.");
        }
        var fieldReference = fieldInjector.GetFieldReference(type);
        CheckIsFrozenMethod = checkIsFrozenBuilder.Build(fieldReference);
        type.Methods.Add(CheckIsFrozenMethod);
        return CheckIsFrozenMethod;
    }

    bool IsCheckMethod(MethodDefinition method)
    {
        return method.Name == "CheckIfFrozen" &&
               method.Parameters.Count == 0;
    }
}