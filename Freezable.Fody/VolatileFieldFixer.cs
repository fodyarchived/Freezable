using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

public class VolatileFieldFixer
{
    TypeFinder typeFinder;
    public List<FieldReference> Fields;
    public VolatileFieldFixer(TypeFinder typeFinder)
    {
        this.typeFinder = typeFinder;
        Fields = new List<FieldReference>();
    }

    public void Execute(List<TypeDefinition> allClasses)
    {
        foreach (var typeDefinition in allClasses)
        {
            ProcessType(typeDefinition);
        }
    }

    void ProcessType(TypeDefinition typeDefinition)
    {
        foreach (var methodDefinition in typeDefinition.Methods)
        {
            if (methodDefinition.IsAbstract)
            {
                continue;
            }
            if (methodDefinition.Body == null)
            {
                continue;
            }
            ProcessBody(methodDefinition.Body);
        }
    }

    void ProcessBody(MethodBody body)
    {
        var instructions = body.Instructions;
        for (var i = 0; i < instructions.Count; i++)
        {
            var instruction = instructions[i];
            if (instruction.OpCode != OpCodes.Stfld && instruction.OpCode != OpCodes.Ldfld)
            {
                continue;
            }

            if (!(instruction.Operand is FieldReference fieldReference))
            {
                continue;
            }
            if (fieldReference.FieldType != typeFinder.VolatileReference)
            {
                continue;
            }
            instructions.Insert(i,Instruction.Create(OpCodes.Volatile));
            i++;
        }
    }
}