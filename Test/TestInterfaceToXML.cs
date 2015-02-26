using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Xml;
using System.IO;

namespace Test
{
    public interface IPerson
    {
        string FirstName { get; set; }
    }

    public class Person : IPerson
    {
        public string FirstName
        {
            get;
            set;
        }
    }

    [TestClass]
    public class TestInterfaceToXML
    {
        [TestMethod]
        public void Serialize()
        {
            var lista = new List<IPerson>();

            lista.Add(new Person { FirstName = "Przemek" });
            lista.Add(new Person { FirstName = "Jola" });

            InterfaceToXML.XMLInterfaceSerialization.Serialize<IPerson>(lista, XmlWriter.Create("test.xml"));

            const string endResult = "<?xml version=\"1.0\" encoding=\"utf-8\"?><IPersonRoot><IPerson><FirstName>Przemek</FirstName></IPerson><IPerson><FirstName>Jola</FirstName></IPerson></IPersonRoot>";

            StreamReader sr = new StreamReader("test.xml");

            var actualResult = sr.ReadLine();

            Assert.AreEqual(endResult, actualResult);
        }
    }
}
