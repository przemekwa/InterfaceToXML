
namespace InterfaceToXML
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Threading;
    using System.Xml;

    public static class XMLInterfaceSerialization
    {
        public static void Serialize<T>(IEnumerable<T> list, XmlWriter xml)
        {
            var interfaceType = list.AsQueryable().ElementType;

            if (!interfaceType.IsInterface)
            {
                throw new ArgumentException("Generic arguments is not a interface");
            }

            var interfaceName = interfaceType.Name;

            xml.WriteStartElement(interfaceName+"Root");

            foreach (var obj in list)
            {
                xml.WriteStartElement(interfaceName);

                foreach (var p in obj.GetType().GetInterface(interfaceName).GetProperties())
                {
                    xml.WriteElementString(p.Name, p.GetValue(obj, null).ToString());
                }

                xml.WriteEndElement();
            }

            xml.Close();
        }

        public static IEnumerable<object> Deserialize(Type interfaceType, string xmlFileName)
        {
            if (!interfaceType.IsInterface)
            {
                throw new ArgumentException("Generic arguments is not a interface");
            }

            var aName = new AssemblyName("temp");

            var appDomain = Thread.GetDomain();

            var aBuilder = appDomain.DefineDynamicAssembly(aName, AssemblyBuilderAccess.RunAndSave);

            var mBuilder = aBuilder.DefineDynamicModule(aName.Name, aName + ".dll");

            TypeBuilder tBuilder = mBuilder.DefineType("TempClass", TypeAttributes.Class | TypeAttributes.Public);

            tBuilder.AddInterfaceImplementation(interfaceType);

            foreach (var propertyInfo in interfaceType.GetProperties())
            {
                PropertyBuilder property = tBuilder.DefineProperty(propertyInfo.Name, PropertyAttributes.HasDefault, CallingConventions.Any, propertyInfo.PropertyType, null);
                MethodBuilder getMethod = tBuilder.DefineMethod("get_" + propertyInfo.Name, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Virtual, propertyInfo.PropertyType, Type.EmptyTypes);
                FieldBuilder privateFiled = tBuilder.DefineField("_" + propertyInfo.Name.ToLower(), propertyInfo.PropertyType, FieldAttributes.Private);

                ILGenerator methodIlGetGenerator = getMethod.GetILGenerator();

                methodIlGetGenerator.Emit(OpCodes.Ldarg_0);
                methodIlGetGenerator.Emit(OpCodes.Ldfld, privateFiled);
                methodIlGetGenerator.Emit(OpCodes.Ret);

                MethodBuilder setMethod = tBuilder.DefineMethod("set_" + propertyInfo.Name,
                MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.SpecialName | MethodAttributes.HideBySig, null,
                new[] { propertyInfo.PropertyType });

                ILGenerator methodSetGenerator = setMethod.GetILGenerator();

                methodSetGenerator.Emit(OpCodes.Ldarg_0);
                methodSetGenerator.Emit(OpCodes.Ldarg_1);
                methodSetGenerator.Emit(OpCodes.Stfld, privateFiled);
                methodSetGenerator.Emit(OpCodes.Ret);

                property.SetSetMethod(setMethod);
                property.SetGetMethod(getMethod);
            }


            Type retval = tBuilder.CreateType();

            aBuilder.Save(aName.Name + ".dll");

            var xml = new XmlDocument();

            xml.Load(xmlFileName);

            var result = new List<object>();

            var xmlNodeList = xml.SelectNodes("//" + interfaceType.Name);

            if (xmlNodeList == null) return result;

            foreach (XmlNode x in xmlNodeList)
            {
                dynamic t = Activator.CreateInstance(retval);

                foreach (XmlNode c in x.ChildNodes)
                {
                    foreach (var pro in t.GetType().GetInterface(interfaceType.Name).GetProperties())
                    {
                        if (pro.Name == c.Name)
                        {
                            pro.SetValue(t, c.InnerText, null);
                        }
                    }
                }

                result.Add(t);
            }
            return result;
        }
    }
}
