using Freezable;

public class ClassWithExplicitProperty : IProperty, IFreezable
{
    bool isFrozen;

    string IProperty.Property { get; set; }

    public void Freeze()
    {
        isFrozen = true;
    }
}
