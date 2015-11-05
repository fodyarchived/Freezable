using System.Linq;
using Mono.Cecil;

public class FieldInjector
{
    VolatileTypeFinder volatileTypeFinder;
    TypeSystem typeSystem;
    VolatileFieldFixer volatileFieldFixer;

    public FieldInjector(VolatileTypeFinder volatileTypeFinder, TypeSystem typeSystem, VolatileFieldFixer volatileFieldFixer)
    {
        this.volatileTypeFinder = volatileTypeFinder;
        this.typeSystem = typeSystem;
        this.volatileFieldFixer = volatileFieldFixer;
    }

    public FieldReference GetFieldReference(TypeDefinition type)
    {
        var fieldReference = FindField(type);
        var modifierType = new RequiredModifierType(volatileTypeFinder.VolatileReference, typeSystem.Boolean);
        
        if (fieldReference.IsStatic)
        {
            throw new WeavingException($"Field '{fieldReference.FullName}' can not be static.");
        }
        var fieldType = fieldReference.FieldType;

        if (fieldType.Name == "Boolean")
        {
            fieldReference.FieldType = modifierType;
            volatileFieldFixer.Fields.Add(fieldReference);
            return fieldReference;
        }
        if (fieldReference.FieldType.Name != modifierType.Name)
        {
            throw new WeavingException("Incorrect type");
        }

        return fieldReference;
    }

    static FieldDefinition FindField(TypeDefinition type)
    {
        var field = type.Fields.FirstOrDefault(x => x.Name == "isFrozen" || x.Name == "_isFrozen");
        if (field == null)
        {
            throw new WeavingException($"Expected to find field named 'isFrozen' or '_isFrozen' in type '{type.FullName}'.");
        }
        return field;
    }
}