namespace Datalaag {
    public class FileLoader {
        public FileLoader() {

        }

        public string Load(string filename) {
            ///source https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/file-system/how-to-read-from-a-text-file
            ///@"C:\Users\Public\TestFolder\WriteText.txt"
            return System.IO.File.ReadAllText(filename);
        }
    }
}