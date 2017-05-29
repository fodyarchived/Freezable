[![Chat on Gitter](https://img.shields.io/gitter/room/fody/fody.svg?style=flat)](https://gitter.im/Fody)
[![NuGet Status](http://img.shields.io/nuget/v/Freezable.Fody.svg?style=flat)](https://www.nuget.org/packages/Freezable.Fody/)


## This is an add-in for [Fody](https://github.com/Fody/Fody/) 

![Icon](https://raw.github.com/Fody/Freezable/master/Icons/package_icon.png)

Implements the Freezable pattern

[Introduction to Fody](http://github.com/Fody/Fody/wiki/SampleUsage)


## The nuget package

https://nuget.org/packages/Freezable.Fody/

    PM> Install-Package Freezable.Fody
    
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


## Icon

Icon courtesy of [The Noun Project](http://thenounproject.com)
