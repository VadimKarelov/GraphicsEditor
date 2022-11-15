using GraphicsEditor.Modules.Elements;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace GraphicsEditor.Modules
{
    internal static class ElementsSaver
    {
        public static void SavePoints(List<IElement> collection, string path)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            string json = JsonSerializer.Serialize(collection, options);

            using (StreamWriter strW = new StreamWriter(path, false))
            {
                strW.WriteLine(json);
            }
        }

        public static List<IElement> LoadPoints(string path)
        {
            List<IElement> collection = new List<IElement>();

            using (StreamReader strR = new StreamReader(path))
            {
                string json = strR.ReadToEnd();
                collection = JsonSerializer.Deserialize<IElement[]>(json).ToList();
            }

            return collection;
        }
    }
}
