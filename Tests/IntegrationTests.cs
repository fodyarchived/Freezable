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
        beforeAssemblyPath = Path.GetFullPath(Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\..\AssemblyToProcess\bin\Debug\AssemblyToProcess.dll"));
#if (!DEBUG)
        beforeAssemblyPath = beforeAssemblyPath.Replace("Debug", "Release");
#endif

        afterAssemblyPath = beforeAssemblyPath.Replace(".dll", "2.dll");
        File.Copy(beforeAssemblyPath, afterAssemblyPath, true);

        using (var moduleDefinition = ModuleDefinition.ReadModule(beforeAssemblyPath, new ReaderParameters()))
        {
            var weavingTask = new ModuleWeaver
            {
                ModuleDefinition = moduleDefinition,
                AssemblyResolver = new MockAssemblyResolver()
            };

            weavingTask.Execute();
            moduleDefinition.Write(afterAssemblyPath);
        }

        assembly = Assembly.LoadFile(afterAssemblyPath);
    }

    [Test]
    public void Frozen()
    {
        var instance = assembly.GetInstance("ClassToFreeze");
        instance.Freeze();
        var exception = Assert.Throws<InvalidOperationException>(() => instance.Property = "aString");
        Assert.AreEqual("Attempted to modify a frozen instance", exception.Message);
    }

    [Test]
    public void SubClass()
    {
        var instance = assembly.GetInstance("SubClassToFreeze");
        instance.Freeze();
        var exception = Assert.Throws<InvalidOperationException>(() => instance.Property2 = "aString");
        Assert.AreEqual("Attempted to modify a frozen instance", exception.Message);
    }

    [Test]
    public void FrozenSubInterface()
    {
        var instance = assembly.GetInstance("ClassToFreezeSubInterface");
        instance.Freeze();
        var exception = Assert.Throws<InvalidOperationException>(() => instance.Property = "aString");
        Assert.AreEqual("Attempted to modify a frozen instance", exception.Message);
    }

    [Test]
    public void NotFrozen()
    {
        var instance = assembly.GetInstance("ClassToFreeze");
        instance.Property = "aString";
    }

    [Test]
    public void FrozenWithSetProperty()
    {
        var instance = assembly.GetInstance("ClassWithSetProperty");
        instance.Freeze();
        var exception = Assert.Throws<InvalidOperationException>(() => instance.Property = "aString");
        Assert.AreEqual("Attempted to modify a frozen instance", exception.Message);
    }

    [Test]
    public void NotFrozenWithSetProperty()
    {
        var instance = assembly.GetInstance("ClassWithSetProperty");
        instance.Property = "aString";
    }

    [Test]
    public void FrozenWithExplicitProperty()
    {
        var instance = assembly.GetInstance("ClassWithExplicitProperty");
        assembly.GetType("IProperty", true).InvokeMember("Property",
            BindingFlags.SetProperty, Type.DefaultBinder, instance, new object[] { "aString" });
    }

    [Test]
    public void NotFrozenWithExplicitProperty()
    {
        var instance = assembly.GetInstance("ClassWithExplicitProperty");
        instance.Freeze();

        Assert.That(() => assembly.GetType("IProperty", true).InvokeMember("Property",
            BindingFlags.SetProperty, Type.DefaultBinder, instance, new object[] { "aString" }),
            Throws.InnerException.TypeOf<InvalidOperationException>()
            .And.InnerException.Message.EqualTo("Attempted to modify a frozen instance"));
    }

#if(DEBUG)
    [Test]
    public void PeVerify()
    {
        Verifier.Verify(beforeAssemblyPath,afterAssemblyPath);
    }
#endif

}