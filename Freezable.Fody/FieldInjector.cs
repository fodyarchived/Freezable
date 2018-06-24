using System.Linq;
using Fody;
using Mono.Cecil;
using TypeSystem = Fody.TypeSystem;

public class FieldInjector
{
    TypeFinder typeFinder;
    TypeSystem typeSystem;
    VolatileFieldFixer volatileFieldFixer;

    public FieldInjector(TypeFinder typeFinder, TypeSystem typeSystem, VolatileFieldFixer volatileFieldFixer)
    {
        this.typeFinder = typeFinder;
        this.typeSystem = typeSystem;
        this.volatileFieldFixer = volatileFieldFixer;
    }

    public FieldReference GetFieldReference(TypeDefinition type)
    {
        var fieldReference = FindField(type);
        var modifierType = new RequiredModifierType(typeFinder.VolatileReference, typeSystem.BooleanReference);

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
        if (field != null)
        {
            return field;
        }
        throw new WeavingException($"Expected to find field named 'isFrozen' or '_isFrozen' in type '{type.FullName}'.");
    }
}