# InterfaceToXML
Biblioteka serializuje i deserializuje interfejsy w C#

Biblioteka jest odpowiedzia na pytanie czy można serializować interfejsy w C#. Cały artykuł dotyczcy tego tematu znajduje sie http://blogprogramisty.net/czy-mozna-serializowac-interfejsy-c-mozna/ 

Biblioteka ma na celu pokazac, że:

* można serializować interfejsy
* można tworzyć dynamiczne typy danych. Czyli taki typy, które powstaju już w trakcie działania programu. Biblioteka pokazuje jak można używać kodu pośredniego do tworzenia właściwości i poł w klasie.


##Szybki start

Biblioteka InterfaceToXML składa się z 2 metod statycznych w klasie statycznej XMLInterfaceSerialization

1. Serialize<T>(IEnumerable<T> list, XmlWriter xml) - zajmuje się serializacją listy do prostego xml-a. UWAGA: Projekt ma charakter edykacyjny i opiera się na serializacji prosrych typów danych.

Przykład użycia (wszystkie przykłady tutaj są dostepne w bibliotece w projekcie Testy)

Jeśli mamy taki interfejs:
```
 public interface IPerson
    {
        string FirstName { get; set; }
    }
```
i taka listę var list =  new List<IPerson>();

To możemy ją serializować do postaci:
```
<?xml version=\"1.0\" encoding=\"utf-8\"?>
<IPersonRoot>
  <IPerson>
    <FirstName>Przemek</FirstName>
  </IPerson>
  <IPerson>
    <FirstName>Jola</FirstName>
  </IPerson>
</IPersonRoot>
```
2. XMLInterfaceSerialization.Deserialize(typeof(IPerson), XMLFILENAME) - metoda zajume się deserializacją pliku xml do postaci listy obiektów implementujących interfejs. 

Na wyjściu dostaniemy taką listę:
```
  this.list = new List<IPerson>();
            
    list.Add(new object { FirstName = "Przemek" });
    list.Add(new object { FirstName = "Jola" });
```
UWAGA: Cały myk polega na tym, że zakładamy, że przed serializacją nie mamy dostępu do obiektu, który implementuje interfejs.

            

    


    


   
