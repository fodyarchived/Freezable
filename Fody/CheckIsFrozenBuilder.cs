using Mono.Cecil;
using Mono.Cecil.Cil;

public class CheckIsFrozenBuilder
{
    TypeSystem typeSystem;
    ExceptionFinder exceptionFinder;

    public CheckIsFrozenBuilder(TypeSystem typeSystem, ExceptionFinder exceptionFinder)
    {
        this.typeSystem = typeSystem;
        this.exceptionFinder = exceptionFinder;
    }

    public MethodDefinition Build(FieldReference fieldReference)
    {
        var isFrozenMethod = new MethodDefinition("CheckIfFrozen", MethodAttributes.Family, typeSystem.Void)
                                 {
                                     IsHideBySig = true,
                                 };
        var ret = Instruction.Create(OpCodes.Ret);
        var instructions = isFrozenMethod.Body.Instructions;
        instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
        if (fieldReference.FieldType is RequiredModifierType)
        {
            instructions.Add(Instruction.Create(OpCodes.Volatile));
        }
        instructions.Add(Instruction.Create(OpCodes.Ldfld, fieldReference));
        instructions.Add(Instruction.Create(OpCodes.Brfalse_S, ret));
        instructions.Add(Instruction.Create(OpCodes.Ldstr, "Attempted to modify a frozen instance"));
        instructions.Add(Instruction.Create(OpCodes.Newobj, exceptionFinder.ExceptionConstructorReference));
        instructions.Add(Instruction.Create(OpCodes.Throw));
        instructions.Add(ret);
        return isFrozenMethod;
    }
}