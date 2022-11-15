using GraphicsEditor.Modules.Elements;
using GraphicsEditor.Modules.Saving;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace GraphicsEditor.Modules
{
    internal static class ElementsSaver
    {
        public static void SaveElements(List<IElement> collection, string path)
        {
            List<SIElement> preparedElements = collection.Select(x => SIElement.GetSElement(x)).ToList();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<SIElement>));

            using (StreamWriter strW = new StreamWriter(path, false))
            {
                xmlSerializer.Serialize(strW, preparedElements);
            }
        }

        public static List<IElement> LoadElements(Camera camera, string path)
        {
            List<IElement> collection = new List<IElement>();

            XmlSerializer formatter = new XmlSerializer(typeof(SIElement[]));

            using (StreamReader strR = new StreamReader(path))
            {
                collection = (formatter.Deserialize(strR) as SIElement[]).Select(x => x.GetRealElement(camera)).ToList();
            }

            return collection;
        }
    }
}
