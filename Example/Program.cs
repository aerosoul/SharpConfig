using System;
using System.Collections.Generic;
using System.Text;
using SharpConfig;

namespace Example
{
    class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public int SomeField;

        // This field will be ignored by SharpConfig
        // when creating sections from objects and vice versa.
        [SharpConfig.Ignore]
        public int SomeIgnoredField = 0;

        // Same for this property.
        [SharpConfig.Ignore]
        public int SomeIgnoredProperty { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Call other methods in this file here to see their effect.
            HowToLoadAConfig();

            Console.WriteLine("Hi, please take a look at the methods below");
            Console.WriteLine("to get an understanding of how to use SharpConfig!");
            Console.ReadLine();
        }

        /// <summary>
        /// Shows how to load a configuration from a string (our sample config).
        /// The same works for loading files and streams. Just call the appropriate method
        /// such as LoadFromFile and LoadFromStream, respectively.
        /// </summary>
        static void HowToLoadAConfig()
        {
            // Read our example config.
            Configuration cfg = Configuration.LoadFromString(Properties.Resources.SampleCfg);
            
            // Just print all sections and their settings.
            PrintConfig(cfg);
        }

        /// <summary>
        /// Prints all sections and their settings to the console.
        /// </summary>
        /// <param name="cfg">The configuration to print.</param>
        static void PrintConfig(Configuration cfg)
        {
            foreach (Section section in cfg)
            {
                Console.WriteLine("Section: " + section.Name);
                Console.WriteLine("Settings:");

                foreach (Setting setting in section)
                {
                    Console.Write("    ");

                    if (setting.IsArray)
                    {
                        Console.Write("[Array, {0} elements] ", setting.ArraySize);
                    }

                    Console.WriteLine(setting.ToString());
                }

                Console.WriteLine();
            }
        }

        /// <summary>
        /// Shows how to create a configuration in memory.
        /// </summary>
        static void HowToCreateAConfig()
        {
            Configuration cfg = new Configuration();

            cfg["Person"]["Name"].SetValue("Peter");
            cfg["Person"]["Age"].SetValue(50);
            cfg["Person"]["SomeField"].SetValue(10);

            // We can obtain the values directly as strings, ints, floats, or any other (custom) type,
            // as long as the string value of the setting can be converted to the type you wish to obtain.
            string nameValue = cfg["Person"]["Name"].StringValue;
            int ageValue = cfg["Person"]["Age"].IntValue;

            // Print our config just to see that it works.
            PrintConfig(cfg);
        }

        /// <summary>
        /// Shows how to save a configuration to a file.
        /// </summary>
        /// <param name="filename">The destination filename.</param>
        static void HowToSaveAConfig(string filename)
        {
            Configuration cfg = new Configuration();

            cfg["Person"]["Name"].SetValue("Peter");
            cfg["Person"]["Age"].SetValue(50);
            cfg["Person"]["SomeField"].SetValue(10);

            cfg.SaveToFile(filename);
        }

        /// <summary>
        /// Shows how to create .NET objects directly from sections.
        /// </summary>
        static void HowToCreateObjectsFromSections()
        {
            Configuration cfg = new Configuration();

            // Create the section.
            cfg["Person"]["Name"].SetValue("Peter");
            cfg["Person"]["Age"].SetValue(50);
            cfg["Person"]["SomeField"].SetValue(10);

            // Now create an object from it.
            Person p = cfg["Person"].CreateObject<Person>();

            // Test.
            Console.WriteLine("Name:      " + p.Name);
            Console.WriteLine("Age :      " + p.Age);
            Console.WriteLine("SomeField: " + p.SomeField);
        }

        /// <summary>
        /// Shows how to create sections directly from .NET objects.
        /// </summary>
        static void HowToCreateSectionsFromObjects()
        {
            Configuration cfg = new Configuration();

            // Create an object.
            Person p = new Person();
            p.Name = "Peter";
            p.Age = 50;
            p.SomeField = 10;

            // Now create a section from it.
            Section section = Section.FromObject("Person Section", p);
            cfg.Add(section);

            // Print the config to see that it worked.
            PrintConfig(cfg);
        }

        /// <summary>
        /// Shows the usage of arrays in SharpConfig.
        /// </summary>
        static void HowToHandleArrays()
        {
            int[] someIntValues = new int[] { 1, 2, 3, 4 };

            Configuration cfg = new Configuration();

            cfg["GeneralSection"]["SomeInts"].SetValue(someIntValues);

            // Get the array back.
            int[] someIntValuesBack = cfg["GeneralSection"]["SomeInts"].GetValueArray<int>();
            float[] sameValuesButFloats = cfg["GeneralSection"]["SomeInts"].GetValueArray<float>();
            string[] sameValuesButStrings = cfg["GeneralSection"]["SomeInts"].GetValueArray<string>();

            // There is also a non-generic variant of GetValueArray:
            object[] sameValuesButObjects = cfg["GeneralSection"]["SomeInts"].GetValueArray(typeof(int));

            PrintArray("someIntValuesBack", someIntValuesBack);
            PrintArray("sameValuesButFloats", sameValuesButFloats);
            PrintArray("sameValuesButStrings", sameValuesButStrings);
            PrintArray("sameValuesButObjects", sameValuesButObjects);
        }

        // Small helper method.
        static void PrintArray<T>(string arrName, T[] arr)
        {
            Console.Write(arrName + " = { ");

            for (int i = 0; i < arr.Length-1; i++)
            {
                Console.Write(arr[i].ToString() + ", ");
            }

            Console.Write(arr[arr.Length - 1].ToString());
            Console.WriteLine(" }");
        }
    }
}
