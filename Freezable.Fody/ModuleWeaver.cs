using System;
using System.Linq;
using Mono.Cecil;

public class ModuleWeaver
{
    public Action<string> LogInfo { get; set; }
    public ModuleDefinition ModuleDefinition { get; set; }
    public IAssemblyResolver AssemblyResolver { get; set; }

    public ModuleWeaver()
    {
        LogInfo = s => { };
    }

    public void Execute()
    {
        var freezableTypeFinder = new FreezableTypeFinder(ModuleDefinition, AssemblyResolver);
        freezableTypeFinder.Execute();

        var typeFinder = new TypeFinder(ModuleDefinition, AssemblyResolver, LogInfo);
        typeFinder.Execute();

        var volatileFieldFixer = new VolatileFieldFixer(typeFinder);
        var fieldInjector = new FieldInjector(typeFinder, ModuleDefinition.TypeSystem, volatileFieldFixer);
        var checkIsFrozenBuilder = new CheckIsFrozenBuilder(ModuleDefinition.TypeSystem, typeFinder);
        var freezeCheckerInjector = new FreezeCheckerInjector(ModuleDefinition, fieldInjector, checkIsFrozenBuilder);

        var typeResolver = new TypeResolver();
        var implementsInterfaceFinder = new ImplementsInterfaceFinder(typeResolver);

        var classes = ModuleDefinition.GetTypes()
            .Where(x => x.IsClass)
            .ToList();
        var assemblyProcessor = new AssemblyProcessor(freezeCheckerInjector, implementsInterfaceFinder, LogInfo);
        assemblyProcessor.Execute(classes);

        volatileFieldFixer.Execute(classes);
    }
}