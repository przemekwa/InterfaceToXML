using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Collections;
using System.Linq;

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
        const string XMLFILENAME = "test.xml";

        [TestMethod]
        public void Serialize()
        {
            var lista = new List<IPerson>();

            lista.Add(new Person { FirstName = "Przemek" });
            lista.Add(new Person { FirstName = "Jola" });

            InterfaceToXML.XMLInterfaceSerialization.Serialize<IPerson>(lista, XmlWriter.Create(XMLFILENAME));

            const string endResult = "<?xml version=\"1.0\" encoding=\"utf-8\"?><IPersonRoot><IPerson><FirstName>Przemek</FirstName></IPerson><IPerson><FirstName>Jola</FirstName></IPerson></IPersonRoot>";

            StreamReader sr = new StreamReader(XMLFILENAME);

            var actualResult = sr.ReadLine();

            Assert.AreEqual(endResult, actualResult);
        }


        [TestMethod]
        public void Deserialize()
        {
            var endResult = new List<IPerson>();

            endResult.Add(new Person { FirstName = "Przemek" });
            endResult.Add(new Person { FirstName = "Jola" });

            var actualResult = InterfaceToXML.XMLInterfaceSerialization.Deserialize(typeof(IPerson), XMLFILENAME);

            Assert.AreEqual(endResult.Count, endResult.Count);

            foreach (var end in endResult)
            {
                Assert.AreEqual(true, actualResult.Cast<IPerson>().Any(l => l.FirstName == end.FirstName));
            }


            
        }
    }
}
