using Logica;
using System.Linq.Expressions;

internal class Program
{
    private static void Main(string[] args)
    {
        ///https://stackoverflow.com/questions/70665003/what-is-the-c-sharp-equivalent-of-java-util-function-supplier
        Func<char> input = () => { Console.Write("input requested: ");return (char)Console.Read();};
        Action<string> output = write => { Console.WriteLine(write); }; 
        BfInterpreter bfinterpreter = new BfInterpreter(
            input,
            output
        );
        bfinterpreter.loadProgram(@"C:\Users\User\Desktop\program.bf");
        bfinterpreter.interpret();
    }
}