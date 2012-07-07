public class ClassWithSetProperty : IFreezable
{
    string property;
    bool isFrozen;

    public string Property
    {
        set { property = value; }
    }

    public void Freeze()
    {
        isFrozen = true;
    }
}