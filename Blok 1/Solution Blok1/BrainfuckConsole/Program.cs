using Logica;
using System.Linq.Expressions;

internal class Program
{
    private static void Main(string[] args)
    {

        
        Func<char> input = () => { Console.WriteLine("input requested: ");return (char)Console.Read();};
        Action<string> output = write => { Console.WriteLine(write); }; 
        BfInterpreter bfinterpreter = new BfInterpreter(
            input,
            output
        );
        bfinterpreter.loadProgram(@"C:\Users\User\Desktop\program.bf");
        bfinterpreter.interpret();
    }
}