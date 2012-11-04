using Freezable;

public class ClassWithAbstractProperty : IFreezable
{
    bool isFrozen;
    public string Property { get; set; }
    public void Freeze()
    {
        isFrozen = true;
    }
}