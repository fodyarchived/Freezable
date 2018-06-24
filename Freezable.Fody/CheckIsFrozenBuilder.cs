using Mono.Cecil;
using Mono.Cecil.Cil;
using TypeSystem = Fody.TypeSystem;

public class CheckIsFrozenBuilder
{
    TypeSystem typeSystem;
    TypeFinder typeFinder;

    public CheckIsFrozenBuilder(TypeSystem typeSystem, TypeFinder typeFinder)
    {
        this.typeSystem = typeSystem;
        this.typeFinder = typeFinder;
    }

    public MethodDefinition Build(FieldReference fieldReference)
    {
        var isFrozenMethod = new MethodDefinition("CheckIfFrozen", MethodAttributes.Family, typeSystem.VoidReference)
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
        instructions.Add(Instruction.Create(OpCodes.Newobj, typeFinder.ExceptionConstructorReference));
        instructions.Add(Instruction.Create(OpCodes.Throw));
        instructions.Add(ret);
        return isFrozenMethod;
    }
}