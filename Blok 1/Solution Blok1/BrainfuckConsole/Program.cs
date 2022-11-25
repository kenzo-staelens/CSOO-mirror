using Logica;
using System.Linq.Expressions;

internal class Program
{
    private static void Main(string[] args)
    {
        Action<string> input = read => { Console.WriteLine("input requested: ");Console.Read();};
        Action<string> output = write => { Console.WriteLine(write); }; 
        BfInterpreter bfinterpreter = new BfInterpreter(
            input,
            output
        );
        bfinterpreter.loadProgram(@"./program.bf");
        bfinterpreter.interpret();
    }
}