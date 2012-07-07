using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

public class VolatileFieldFixer
{
    VolatileTypeFinder volatileTypeFinder;
    public List<FieldReference> Fields;
    public VolatileFieldFixer(VolatileTypeFinder volatileTypeFinder)
    {
        this.volatileTypeFinder = volatileTypeFinder;
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
            if (instruction.OpCode == OpCodes.Stfld || instruction.OpCode == OpCodes.Ldfld)
            {
                var fieldReference = instruction.Operand as FieldReference;
                if (fieldReference == null)
                {
                    continue;
                }
                if (fieldReference.FieldType != volatileTypeFinder.VolatileReference)
                {
                    continue;
                }
                instructions.Insert(i,Instruction.Create(OpCodes.Volatile));
                i++;
            }
        }
    }
}