using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Collections;
using System.Xml;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

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

        List<IPerson> list;

        [TestInitialize]
        public void Initialize()
        {
            this.list = new List<IPerson>();
            
            list.Add(new Person { FirstName = "Przemek" });
            list.Add(new Person { FirstName = "Jola" });
        }

        [TestMethod]
        public void Serialize()
        {
            const string correctFile = "<?xml version=\"1.0\" encoding=\"utf-8\"?><IPersonRoot><IPerson><FirstName>Przemek</FirstName></IPerson><IPerson><FirstName>Jola</FirstName></IPerson></IPersonRoot>";

            InterfaceToXML.XMLInterfaceSerialization.Serialize<IPerson>(list, XmlWriter.Create(XMLFILENAME));

            StreamReader sr = new StreamReader(XMLFILENAME);

            var resultFile = sr.ReadLine();

            Assert.AreEqual(correctFile, resultFile);
        }


        [TestMethod]
        public void Deserialize()
        {
            var actualResult = InterfaceToXML.XMLInterfaceSerialization.Deserialize(typeof(IPerson), XMLFILENAME);

            
            foreach (var end in list)
            {
                Assert.AreEqual(true, actualResult.Cast<IPerson>().Any(l => l.FirstName == end.FirstName));
            }
        }


        [TestMethod]
        [ExpectedException(typeof(NotSupportedException),"Cannot serialize interface Test.IPerson.")]
        public void SerializerError()
        {
            XmlSerializer xml = new XmlSerializer(typeof(List<IPerson>));

            xml.Serialize(XmlWriter.Create(XMLFILENAME), list);

        }
    }
}
