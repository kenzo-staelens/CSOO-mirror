using System.ComponentModel;

namespace Datalaag {
    public class FileLoader {
        public FileLoader() {}

        /// <summary>
        /// laad text data van een file
        /// </summary>
        /// <param name="filename"></param>
        /// <returns>string file data</returns>
        /// <see cref="https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/file-system/how-to-read-from-a-text-file"/>
        public string Load(string filename) {
            if (!File.Exists(filename)) throw new IOException("file not found");
            if ((new FileInfo(filename)).Length > 100000) throw new IOException("file too large");
            return System.IO.File.ReadAllText(filename);
        }

        public void Save(string filename,string text) {
            if (!File.Exists(filename)) File.Create(filename).Close();
            using (StreamWriter writer = new StreamWriter(filename, false)) {
                writer.Write(text);
            }
        }
    }
}
