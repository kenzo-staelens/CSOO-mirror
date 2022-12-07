using System.Runtime.CompilerServices;
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
    public struct Programdata {

        public Commands[] Compiled { get; private set; }
        public int Length { get; private set; }
        public Programdata(string code) {
            code = Regex.Replace(code, "[^\\+\\-<>\\.\\,\\[\\]]", "");
            this.Compiled = new Commands[code.Length];
            for (int i = 0; i < code.Length; i++) {
                this.Compiled[i] = InstructionMap[code[i]];
            }
            this.Length = this.Compiled.Length;
        }

        public string Serialize() {
            string returnstr = "";
            for(int i = 0; i < Compiled.Length; i++) {
                returnstr += ReverseInstructionMap[Compiled[i]];
            }
            return returnstr;
        }

        private static readonly Dictionary<char, Commands> InstructionMap = new Dictionary<char, Commands>() {
            { '+', Commands.Inc},
            { '-', Commands.Dec},
            { '<', Commands.Left},
            { '>', Commands.Right},
            { '[', Commands.Loop},
            { ']', Commands.Jmp},
            { '.', Commands.Write},
            { ',', Commands.Read},
        };

        private static readonly Dictionary<Commands, char> ReverseInstructionMap = new Dictionary<Commands, char>() {
            {Commands.Inc, '+'},
            {Commands.Dec, '-'},
            {Commands.Left, '<'},
            {Commands.Right, '>'},
            {Commands.Loop, '['},
            {Commands.Jmp, ']'},
            {Commands.Write, '.'},
            {Commands.Read, ','},
        };
    }
}