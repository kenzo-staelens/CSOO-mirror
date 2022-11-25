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
            return System.IO.File.ReadAllText(filename);
        }
    }
}