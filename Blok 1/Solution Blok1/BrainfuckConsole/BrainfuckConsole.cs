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
        //setup
        Func<char> Input = () => { Console.Write("input requested: "); return (char)Console.Read(); };
        Action<string> Output = write => { Console.Write(write); };
        
        BfInterpreter bfinterpreter = new BfInterpreter(Input, Output);
        Action tick = () => { };// { Console.WriteLine(bfinterpreter.Program.Serialize()); Console.WriteLine(new string(' ',bfinterpreter.ProgramPointer)+"^");};
        bfinterpreter.Tick = tick;

        string? input = "";
        //user input
        while (input==null || input.Equals("")) {
            Console.WriteLine("Geef een pad of brainfuck programma mee of \"help\": ");
            input = Console.ReadLine();
            if (input==null || input.ToLower().Equals("help")) {
                Console.WriteLine(
@"Paden
    .\pad\vanaf\huidige\directory
    C:\absoluut\pad\in\c\schijf
directe code (print a)
    >+++++++++++[-<++++++>]<-.
instructieset
    + increment cell
    - decrement cell
    > move left
    < move right
    [ start loop
    ] end loop
    . read input
    , write input");
                input = "";
            }
        }
        //bfinterpreter.LoadProgram(@"C:\Users\User\Desktop\gitCSharp\Blok 1\Solution Blok1\user defined resources\bfcode_debug_file.txt");
        //C:\Users\User\Desktop\gitCSharp\Blok 1\Solution Blok1\user defined resources\bfcode_debug_file.txt
        //bfinterpreter.LoadProgram(">>>+<<<[-[->+>+<<]>>[-<<+>>]<+[->>[->+>+<<]>[->>+<<]>[-<<+>>]<<<<]>>[-]>>>[-<<<+>>>]<<<<<<]>>>");

        bfinterpreter.LoadProgram(input);
        bfinterpreter.PreparedInput = "";
        bfinterpreter.Interpret();
    }
}
