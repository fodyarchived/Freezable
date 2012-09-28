using System;
using System.IO;
using System.Reflection;
using Mono.Cecil;
using NUnit.Framework;

[TestFixture]
public class IntegrationTests
{
    Assembly assembly;

    public IntegrationTests()
    {
        var assemblyPath = Path.GetFullPath(@"..\..\..\AssemblyToProcess\bin\Debug\AssemblyToProcess.dll");
#if (!DEBUG)

        assemblyPath = assemblyPath.Replace("Debug", "Release");
#endif

        var newAssembly = assemblyPath.Replace(".dll", "2.dll");
        File.Copy(assemblyPath, newAssembly, true);

        var moduleDefinition = ModuleDefinition.ReadModule(newAssembly);
        var weavingTask = new ModuleWeaver
                              {
                                  ModuleDefinition = moduleDefinition,
                                  AssemblyResolver = new MockAssemblyResolver()
                              };

        weavingTask.Execute();
        moduleDefinition.Write(newAssembly);

        assembly = Assembly.LoadFile(newAssembly);
    }

    [Test]
    [ExpectedException(typeof(InvalidOperationException), ExpectedMessage = "Attempted to modify a frozen instance")]
    public void Frozen()
    {
        var instance = assembly.GetInstance("ClassToFreeze");
        instance.Freeze();
        instance.Property = "fsdf";
    }

    [Test]
    [ExpectedException(typeof(InvalidOperationException), ExpectedMessage = "Attempted to modify a frozen instance")]
    public void SubClass()
    {
        var instance = assembly.GetInstance("SubClassToFreeze");
        instance.Freeze();
        instance.Property2 = "fsdf";
    }

    [Test]
    [ExpectedException(typeof(InvalidOperationException), ExpectedMessage = "Attempted to modify a frozen instance")]
    public void FrozenSubInterface()
    {
        var instance = assembly.GetInstance("ClassToFreezeSubInterface");
        instance.Freeze();
        instance.Property = "fsdf";
    }
    
    [Test]
    public void NotFrozen()
    {
        var instance = assembly.GetInstance("ClassToFreeze");
        instance.Property = "fsdf";
    }
    
    [Test]
    [ExpectedException(typeof(InvalidOperationException), ExpectedMessage = "Attempted to modify a frozen instance")]
    public void FrozenWithSetProperty()
    {
        var instance = assembly.GetInstance("ClassWithSetProperty");
        instance.Freeze();
        instance.Property = "fsdf";
    }

    [Test]
    public void NotFrozenWithSetProperty()
    {
        var instance = assembly.GetInstance("ClassWithSetProperty");
        instance.Property = "fsdf";
    }

#if(DEBUG)
    [Test]
    public void PeVerify()
    {
        Verifier.Verify(assembly.CodeBase.Remove(0, 8));
    }
#endif

}