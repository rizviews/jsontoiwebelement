using System;
using CommandLineParser.Arguments;
using System.IO;

namespace Ridia.TestAutomation
{
    class Program
    {
        static void Main(string[] args)
        {
            CommandLineParser.CommandLineParser parser = new CommandLineParser.CommandLineParser();
            ValueArgument<string> directory = new ValueArgument<string>('d', "direcotry", "Set element definition directory");
            ValueArgument<string> defaultNameSpace = new ValueArgument<string>('n', "NameSpace", "Set the default name space for generated classes");

            parser.Arguments.Add(directory);
            parser.Arguments.Add(defaultNameSpace);

            parser.ParseCommandLine(args);

            Console.WriteLine("Passed argument is: "+directory.Value);
            // access the values 
            if (directory.Parsed) /// test, whether the argument appeared on the command line
            {
                var dir = directory.Value;
                JsonToNativeElementsConverter jsonToNativeElementsConverter = new JsonToNativeElementsConverter();
                foreach (string file in Directory.GetFiles(dir,"*.json"))
                {
                    Console.WriteLine($"Processing {file}");
                    jsonToNativeElementsConverter.ToElements(file);
                }
            }
        }
    }
}
