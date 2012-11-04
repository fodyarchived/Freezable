using Freezable;

public class ClassToFreeze : IFreezable
{
    bool isFrozen;
    public string Property { get; set; }
    public void Freeze()
    {
        isFrozen = true;
    }
}