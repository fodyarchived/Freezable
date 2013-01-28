using System;
using System.IO;
using System.Reflection;
using Mono.Cecil;
using NUnit.Framework;

[TestFixture]
public class IntegrationTests
{
    Assembly assembly;
    string beforeAssemblyPath;
    string afterAssemblyPath;

    public IntegrationTests()
    {
        beforeAssemblyPath = Path.GetFullPath(@"..\..\..\AssemblyToProcess\bin\Debug\AssemblyToProcess.dll");
#if (!DEBUG)

        beforeAssemblyPath = beforeAssemblyPath.Replace("Debug", "Release");
#endif

        afterAssemblyPath = beforeAssemblyPath.Replace(".dll", "2.dll");
        File.Copy(beforeAssemblyPath, afterAssemblyPath, true);

        var moduleDefinition = ModuleDefinition.ReadModule(afterAssemblyPath, new ReaderParameters
        {
        });
        var weavingTask = new ModuleWeaver
        {
            ModuleDefinition = moduleDefinition,
            AssemblyResolver = new MockAssemblyResolver()
        };

        weavingTask.Execute();
        moduleDefinition.Write(afterAssemblyPath);

        assembly = Assembly.LoadFile(afterAssemblyPath);
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
        Verifier.Verify(beforeAssemblyPath,afterAssemblyPath);
    }
#endif

}