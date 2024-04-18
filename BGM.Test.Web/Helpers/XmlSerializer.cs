using System.Xml;
using System.Xml.Serialization;

namespace BGM.Test.Web.Helpers;

public static class XmlSerializer<T> where T : class
{
    public static void Serialize(XmlWriter writer, T obj) =>
        new XmlSerializer(typeof(T)).Serialize(writer, obj);
    
    public static T? Deserialize(StreamReader reader) => 
        new XmlSerializer(typeof(T)).Deserialize(reader) as T;
}