![sharpconfig_logo.png](sharpconfig_logo.png)

SharpConfig is an easy-to-use CFG/INI configuration library for .NET.

You can use SharpConfig in your .NET applications to add the functionality
to read, modify and save configuration files and streams, in either text or binary format.

> If SharpConfig has helped you and you feel like donating, [feel free](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=WWN94LMDN5HMC)!
> Donations help to keep the development of SharpConfig active.

Installing via NuGet
---
You can install SharpConfig via the following NuGet command:
> Install-Package sharpconfig
[NuGet Page](https://www.nuget.org/packages/sharpconfig/)



An example Configuration
---

```cfg
[General]
# a comment
SomeString = Hello World!
SomeInteger = 10 # an inline comment
SomeFloat = 20.05
ABoolean = true
```

To read these values, your C# code would look like:
```csharp
Configuration config = Configuration.LoadFromFile("sample.cfg");
Section section = config["General"];

string someString = section["SomeString"].StringValue;
int someInteger = section["SomeInteger"].IntValue;
float someFloat = section["SomeFloat"].FloatValue.
```

Enumerations
---

SharpConfig is also able to parse enumerations.
For example you have a configuration like this:
```cfg
[DateInfo]
Day = Monday
```

It is now possible to read this value as a System.DayOfWeek enum, because **Monday** is present there.
An example of how to read it:

```csharp
DayOfWeek day = config["DateInfo"]["Day"].GetValueTyped<DayOfWeek>();
```

Arrays
---

Arrays are also supported in SharpConfig.
For example you have a configuration like this:
```cfg
[General]
MyArray = {0,2,5,6}
```

This array can be interpreted as any type array that can be converted from 0, 2, 5 and 6, for example int, float, double, char, byte, string etc.

Reading this array is simple:
```csharp
string[] stringArray = config["General"]["MyArray"].GetValueArray<string>();
int[] intArray = config["General"]["MyArray"].GetValueArray<int>();
// ...
```

Creating a Configuration in-memory
---

```csharp
// Create the configuration.
var myConfig = new Configuration();

// Set some values.
// This will automatically create the sections and settings.
myConfig["Video"]["Width"].IntValue = 1920;
myConfig["Video"]["Height"].IntValue = 1080;

// Set an array value.
myConfig["Video"]["Formats"].SetValue( new string[] { "RGB32", "RGBA32" } );

// Get the values just to test.
int width = myConfig["Video"]["Width"].IntValue;
int height = myConfig["Video"]["Height"].IntValue;
string[] formats = myConfig["Video"]["Formats"].GetValueArray<string>();
// ...
```

Iterating through a Configuration
---

```csharp
foreach (var section in myConfig)
{
    foreach (var setting in section)
    {
        // ...
    }
}
```

Saving a Configuration
---

```csharp
myConfig.SaveToFile("myConfig.cfg");        // Save to a text-based file.
myConfig.SaveToStream(myStream);            // Save to a text-based stream.
myConfig.SaveBinaryToFile("myConfig.cfg");  // Save to a binary file.
myConfig.SaveBinaryToStream(myStream);      // Save to a binary stream.
```

Object Mapping
---

A nice-to-have in SharpConfig is the mapping of sections to objects and vice versa.
If you have a class and enumeration in C# like this:
```csharp
class Person
{
    public string Name { get; set; }
    public int Age { get; set; }
    public Gender Gender { get; set; }
}

enum Gender
{
    Male,
    Female
}
```

It is possible to create an object straight from the configuration file:
```cfg
[Person]
Name = Peter
Age = 50
Gender = Male
```
Like this:
```csharp
Person person = config["Person"].CreateObject<Person>();
```

You can also create a section from objects. Let's create a section out of the
Person object above:
```csharp
Section section = Section.FromObject("Person", person);
```

The first parameter is the name of the section. You can choose this freely, but it must not be empty.
The second parameter specifies the object that is used to create the section.
After this, the section would look exactly like the [Person] section above.

**Note**: Creating objects from sections uses the type's public property getters and setters.
It uses string conversion for every type, so if you use a custom (complex) type, please ensure
that it can be created from a string and also returns an appropriate string from its ToString method.
Otherwise, if you just use primitive types such as int, float, bool, enums or strings, it just works.


If you already have a Person object and don't want to create a new one, you can use the MapTo method:
```csharp
config["Person"].MapTo(person);
```

More examples
---
For more examples and how to use SharpConfig, please look at the [Example application](https://github.com/cemdervis/SharpConfig/blob/master/Example/Program.cs).
