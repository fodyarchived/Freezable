[![Chat on Gitter](https://img.shields.io/gitter/room/fody/fody.svg?style=flat)](https://gitter.im/Fody/Fody)
[![NuGet Status](http://img.shields.io/nuget/v/Freezable.Fody.svg?style=flat)](https://www.nuget.org/packages/Freezable.Fody/)


## This is an add-in for [Fody](https://github.com/Fody/Fody/)

![Icon](https://raw.github.com/Fody/Freezable/master/package_icon.png)

Implements the Freezable pattern

[Introduction to Fody](http://github.com/Fody/Fody/wiki/SampleUsage)


## Usage

See also [Fody usage](https://github.com/Fody/Fody#usage).


### NuGet installation

Install the [Freezable.Fody NuGet package](https://nuget.org/packages/Freezable.Fody/) and update the [Fody NuGet package](https://nuget.org/packages/Fody/):

```
PM> Install-Package Freezable.Fody
PM> Update-Package Fody
```

The `Update-Package Fody` is required since NuGet always defaults to the oldest, and most buggy, version of any dependency.


### Add to FodyWeavers.xml

Add `<Freezable/>` to [FodyWeavers.xml](https://github.com/Fody/Fody#add-fodyweaversxml)

```xml
<?xml version="1.0" encoding="utf-8" ?>
<Weavers>
  <Freezable/>
</Weavers>
```



### Add an interface

```
public interface IFreezable
{
    void Freeze();
}
```


### Add a freezable class

```
public class Person : IFreezable
{
    bool isFrozen;
    public string Name { get; set; }

    public void Freeze()
    {
        isFrozen = true;
    }
}
```


### What gets compiled

```
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
```


## Icon

Icon courtesy of [The Noun Project](http://thenounproject.com)