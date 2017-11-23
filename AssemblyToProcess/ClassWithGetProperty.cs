using Freezable;

public class ClassWithGetProperty : IFreezable
{
    bool isFrozen;

    public string Property => "";    public void Freeze()
    {
        isFrozen = true;
    }
}