using System;

public class PersonWeaved : IFreezable
{

    volatile bool isFrozen;
    public void Freeze()
    {
        isFrozen = true;
    }

    void CheckIfFrozen()
    {
        if (isFrozen)
        {
            throw new InvalidOperationException("Attempted to modify a frozen instance");
        }
    }

    string name;
    public string Name
    {
        get { return name; }
        set
        {
            CheckIfFrozen();
            name = value;
        }
    }

}