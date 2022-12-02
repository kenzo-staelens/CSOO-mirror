using System.Text;
using System.Text.RegularExpressions;

namespace Globals {
    /// <summary>
    /// enumeraties voor valid brainfuck commandos
    /// </summary>
    /// <see cref="https://stackoverflow.com/questions/2373986/how-can-i-use-a-special-char-in-a-c-sharp-enum"/>
    /// <see cref="https://stackoverflow.com/questions/13099834/how-to-get-the-display-name-attribute-of-an-enum-member-via-mvc-razor-code"/>
    public enum Commands {
        Inc,
        Dec,
        Left,
        Right,
        Loop,
        Jmp,
        Write,
        Read,
    }
    public class BrainfuckPrecompiler {

        private static readonly Dictionary<char, Commands> instructionMap = new Dictionary<char, Commands>() {
            { '+', Commands.Inc},
            { '-', Commands.Dec},
            { '>', Commands.Left},
            { '-', Commands.Right},
            { '[', Commands.Loop},
            { ']', Commands.Jmp},
            { '.', Commands.Write},
            { ',', Commands.Read},
        };

        public static Dictionary<char, Commands> InstructionMap { get; }

        public static Commands[] SimpleEncoding(string code) {
            code = Minimal(code);
            var returnvalue = new Commands[code.Length];
            for(int i =0;i<code.Length;i++) {
                returnvalue[i] = instructionMap[code[i]];
            }
            return returnvalue;
        }

        private static string Minimal(string code) {
            return Regex.Replace(code, "[^\\+\\-<>\\.\\,\\[\\]]", "");
        }
    }
}