using System;
using System.Reflection;
using Fody;
using Xunit;
#pragma warning disable 618

public class IntegrationTests
{
    static Assembly assembly;
    static TestResult testResult;

    static IntegrationTests()
    {
        var weavingTask = new ModuleWeaver();
#if(NET46)
        testResult = weavingTask.ExecuteTestRun("AssemblyToProcess.dll");
#else
        testResult = weavingTask.ExecuteTestRun("AssemblyToProcess.dll", runPeVerify: false);
#endif
        assembly = testResult.Assembly;
    }

    [Fact]
    public void Frozen()
    {
        var instance = testResult.GetInstance("ClassToFreeze");
        instance.Freeze();
        var exception = Assert.Throws<InvalidOperationException>(() => instance.Property = "aString");
        Assert.Equal("Attempted to modify a frozen instance", exception.Message);
    }

    [Fact]
    public void SubClass()
    {
        var instance = testResult.GetInstance("SubClassToFreeze");
        instance.Freeze();
        var exception = Assert.Throws<InvalidOperationException>(() => instance.Property2 = "aString");
        Assert.Equal("Attempted to modify a frozen instance", exception.Message);
    }

    [Fact]
    public void FrozenSubInterface()
    {
        var instance = testResult.GetInstance("ClassToFreezeSubInterface");
        instance.Freeze();
        var exception = Assert.Throws<InvalidOperationException>(() => instance.Property = "aString");
        Assert.Equal("Attempted to modify a frozen instance", exception.Message);
    }

    [Fact]
    public void NotFrozen()
    {
        var instance = testResult.GetInstance("ClassToFreeze");
        instance.Property = "aString";
    }

    [Fact]
    public void FrozenWithSetProperty()
    {
        var instance = testResult.GetInstance("ClassWithSetProperty");
        instance.Freeze();
        var exception = Assert.Throws<InvalidOperationException>(() => instance.Property = "aString");
        Assert.Equal("Attempted to modify a frozen instance", exception.Message);
    }

    [Fact]
    public void NotFrozenWithSetProperty()
    {
        var instance = testResult.GetInstance("ClassWithSetProperty");
        instance.Property = "aString";
    }

    [Fact]
    public void FrozenWithExplicitProperty()
    {
        var instance = testResult.GetInstance("ClassWithExplicitProperty");
        assembly.GetType("IProperty", true).InvokeMember("Property",
            BindingFlags.SetProperty, Type.DefaultBinder, instance, new object[]
            {
                "aString"
            });
    }

    [Fact]
    public void NotFrozenWithExplicitProperty()
    {
        var instance = testResult.GetInstance("ClassWithExplicitProperty");
        instance.Freeze();

        var type = assembly.GetType("IProperty", true);
        var exception = Assert.Throws<TargetInvocationException>(() => type.InvokeMember("Property",
            BindingFlags.SetProperty, Type.DefaultBinder, instance, new object[]
            {
                "aString"
            }));
        var innerException = exception.InnerException;
        Assert.IsType<InvalidOperationException>(innerException);
        Assert.Equal("Attempted to modify a frozen instance", innerException.Message);
    }
}