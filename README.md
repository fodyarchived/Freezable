## This is an add-in for [Fody](https://github.com/SimonCropp/Fody/) 

Implements the Freezable pattern

[Introduction to Fody](http://github.com/SimonCropp/Fody/wiki/SampleUsage)

## Nuget package http://nuget.org/packages/Freezable.Fody 

### Add an interface

    public interface IFreezable
    {
        void Freeze();
    }

### Add a freezable class

    public class Person : IFreezable
    {
        bool isFrozen;
        public string Name { get; set; }
    
        public void Freeze()
        {
            isFrozen = true;
        }
    }


### What gets compiled 

    public class Person : IFreezable
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
