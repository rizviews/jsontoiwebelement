namespace Ridia.TestAutomation
{
    using Testura.Code.Generators.Common;
    using Testura.Code.Generators.Common.Arguments.ArgumentTypes;
    using Testura.Code.Statements;
    using Testura.Code.Saver;
    using OpenQA.Selenium;
    using Ridia.TestAutomation.Model;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using System.Collections.Generic;
    using Testura.Code.Models;
    using Testura.Code.Models.References;
    using Testura.Code.Models.Properties;
    using Testura.Code;
    using Testura.Code.Builders;
    using Testura.Code.Generators.Class;
    using System.IO;
    using System;

    public class JsonToNativeElementsConverter
    {
        /// <summary>
        /// To the elements.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public void ToElements(string fileName, string nameSpace)
        {
            JsonToIWebElement jsonToIWebElement = new JsonToIWebElement(fileName);

            var className = System.IO.Path.GetFileNameWithoutExtension(fileName);
            
            Statement.Declaration.Assign("this.webDriver", new VariableReference("webDriver"));
            Statement.Declaration.Assign("this.webDriver", new VariableReference("jsonToWebElement"));

            BlockSyntax blockSyntax =  BodyGenerator.Create(
                Statement.Declaration.Assign("this.webDriver", new VariableReference("webDriver")),
                Statement.Declaration.Assign("this.jsonToWebElement", new VariableReference("jsonToWebElement"))
                );
            ConstructorDeclarationSyntax constructor = ConstructorGenerator.Create(className, blockSyntax, new Parameter[] { new Parameter("webDriver", typeof(IWebDriver)), new Parameter("jsonToWebElement", typeof(JsonToIWebElement)) }, new Modifiers[] { Modifiers.Public });

            List<PageElementModel> elements = jsonToIWebElement.GetPageModels();

            var @class = new ClassBuilder(className, nameSpace)
                        .WithUsings(new string[] { "System", "OpenQA.Selenium", "Ridia.TestAutomation" })
                        .WithModifiers(Modifiers.Public)
                        .WithFields(new Field[] { new Field("webDriver", typeof(IWebDriver), new Modifiers[] { Modifiers.Private }), new Field("jsonToWebElement", typeof(JsonToIWebElement), new Modifiers[] { Modifiers.Private }) })
                        .WithConstructor(constructor);

            Property[] properties = new Property[elements.Count];
            int index = 0;
            foreach (PageElementModel ele in elements)
            {
                properties[index] = new BodyProperty(ele.Name, typeof(IWebElement),
                    BodyGenerator.Create(
                        Statement.Jump.Return(new VariableReference("this.jsonToWebElement",new MethodReference("GetElement",new IArgument[] {new ValueArgument(ele.Name),new VariableArgument("this.webDriver") }))))
                    ,new Modifiers[] {Modifiers.Public });
                index++;
            }

            @class.WithProperties(properties);
            
            var @code = @class.Build();
            var saver = new CodeSaver();
            string path = Path.GetDirectoryName(fileName);
            Console.WriteLine($"Writing generated code to {path}{Path.DirectorySeparatorChar}{className}.cs");
            saver.SaveCodeToFile(@code,path+Path.DirectorySeparatorChar+className +".cs");
        }
    }
}
