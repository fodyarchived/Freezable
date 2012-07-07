public class ClassWithGetProperty : IFreezable
{
    bool isFrozen;

    public string Property
    {
        get { return ""; }
    }

    public void Freeze()
    {
        isFrozen = true;
    }
}