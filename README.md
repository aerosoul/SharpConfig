![sharpconfig_logo.png](sharpconfig_logo.png)

SharpConfig is an easy-to-use CFG/INI configuration library for .NET.

You can use SharpConfig to read, modify and save configuration files and streams, in either text or binary format.

The library is fully compatible with .NET 2.0 and higher, and the Mono Framework.

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
SomeBoolean = true
```

To read these values, your C# code would look like:
```csharp
Configuration config = Configuration.LoadFromFile("sample.cfg");
Section section = config["General"];

string someString = section["SomeString"].StringValue;
int someInteger = section["SomeInteger"].IntValue;
float someFloat = section["SomeFloat"].FloatValue;
bool someBool = section["SomeBoolean"].BoolValue;
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
myConfig["Video"]["Formats"].StringValueArray = new[] { "RGB32", "RGBA32" };

// Get the values just to test.
int width = myConfig["Video"]["Width"].IntValue;
int height = myConfig["Video"]["Height"].IntValue;
string[] formats = myConfig["Video"]["Formats"].StringValueArray;
// ...
```

Saving a Configuration
---

```csharp
myConfig.SaveToFile("myConfig.cfg");        // Save to a text-based file.
myConfig.SaveToStream(myStream);            // Save to a text-based stream.
myConfig.SaveToBinaryFile("myConfig.cfg");  // Save to a binary file.
myConfig.SaveToBinaryStream(myStream);      // Save to a binary stream.
```

More!
---
SharpConfig has more features, such as **arrays**, **enums** and **object mapping**.

For details and examples, please visit the [Wiki](https://github.com/cemdervis/SharpConfig/wiki).
There are also use case examples available in the [Example File](https://github.com/cemdervis/SharpConfig/blob/master/Example/Program.cs).
