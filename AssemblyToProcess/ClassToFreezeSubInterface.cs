public class ClassToFreezeSubInterface : ISubFreezable
{
    bool isFrozen;
    public string Property { get; set; }
    public void Freeze()
    {
        isFrozen = true;
    }
}