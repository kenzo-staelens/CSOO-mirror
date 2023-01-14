using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datalaag {
    public class XML_IO {
        public static void Write(string path, string XmlString) {
            if (!path.EndsWith(".xml")) throw new ArgumentException("expected xml as file extension");
            using (StreamWriter outputFile = new StreamWriter(path, false)) {
                outputFile.Write(XmlString);
            }
        }

        public static string Read(string path) {
            if (!path.EndsWith(".xml")) throw new FileLoadException("expected xml as file extension");
            string text;
            using (StreamReader inputFile = new StreamReader(path)) {
                text = inputFile.ReadToEnd();
            }
            return text;
        }
    }
}
