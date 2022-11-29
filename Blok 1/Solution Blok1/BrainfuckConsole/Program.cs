using Logica;
using System.Linq.Expressions;

internal class Program {
    /// <summary>
    /// main functie
    /// </summary>
    /// <param name="args"></param>
    /// <see cref="https://stackoverflow.com/questions/70665003/what-is-the-c-sharp-equivalent-of-java-util-function-supplier"/>
    /// <see cref="https://stackoverflow.com/questions/36449343/what-is-the-c-sharp-equivalent-of-java-8-java-util-function-consumer"/>
    private static void Main(string[] args) {
        Func<char> Input = () => { Console.Write("input requested: "); return (char)Console.Read(); };
        Action<string> Output = write => { Console.WriteLine(write); };
        BfInterpreter bfinterpreter = new BfInterpreter(
            Input,
            Output
        );
        //bfinterpreter.LoadProgram(@"C:\Users\User\Desktop\program.bf");
        bfinterpreter.LoadProgram(">+++++++++++[-<++++++>]<-.,.");
        bfinterpreter.PrepareInput("b");
        bfinterpreter.Interpret();
    }
}
