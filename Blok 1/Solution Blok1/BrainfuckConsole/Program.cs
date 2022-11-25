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
        Func<char> input = () => { Console.Write("input requested: "); return (char)Console.Read(); };
        Action<string> output = write => { Console.WriteLine(write); };
        BfInterpreter bfinterpreter = new BfInterpreter(
            input,
            output
        );
        //bfinterpreter.loadProgram(@"C:\Users\User\Desktop\program.bf");
        bfinterpreter.loadProgram(">+++++++++++[-<++++++>]<-.,.");
        bfinterpreter.prepareInput("b");
        bfinterpreter.interpret();
    }
}