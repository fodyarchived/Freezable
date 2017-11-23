using System;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using Mono.Collections.Generic;

public class TypeProcessor
{
    Action<string> logInfo;
    MethodReference freezeChecker;
    TypeDefinition type;

    public TypeProcessor(Action<string> logInfo, MethodReference freezeChecker, TypeDefinition type)
    {
        this.logInfo = logInfo;
        this.freezeChecker = freezeChecker;
        this.type = type;
    }

    public void Execute()
    {
        logInfo("\t" + type.FullName);

        foreach (var property in type.Properties)
        {
            if (property.Name == "IsFrozen" || property.Name == "IFreezable.IsFrozen")
            {
                continue;
            }
            var setMethod = property.SetMethod;
            if (setMethod == null)
            {
                continue;
            }
            if (setMethod.IsPrivate && !property.Name.Contains('.'))
            {
                continue;
            }
            if (setMethod.IsAbstract)
            {
                continue;
            }
            ProcessProperty(setMethod);
        }
    }

    void ProcessProperty(MethodDefinition method)
    {
        var instructions = method.Body.Instructions;
        if (AlreadyContainsCheck(instructions))
        {
            return;
        }
        method.Body.SimplifyMacros();
        method.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Callvirt, freezeChecker));
        method.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Ldarg_0));
        method.Body.OptimizeMacros();
    }

    static bool AlreadyContainsCheck(Collection<Instruction> instructions)
    {
        return instructions
            .Select(instruction => instruction.Operand)
            .OfType<MethodReference>()
            .Any(operand => operand.Name == "CheckIsFrozen");
    }
}