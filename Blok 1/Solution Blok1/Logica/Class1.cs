using Datalaag;
using System.ComponentModel;
using System.Reflection;

namespace Logica {
    //source   https://stackoverflow.com/questions/2373986/how-can-i-use-a-special-char-in-a-c-sharp-enum
    //source 2 https://stackoverflow.com/questions/13099834/how-to-get-the-display-name-attribute-of-an-enum-member-via-mvc-razor-code
    public enum commands {
        [Description("+")] Inc,
        [Description("-")] Dec,
        [Description("<")] Left,
        [Description(">")] Right,
        [Description("[")] Loop,
        [Description("]")] Jmp,
        [Description(".")] Write,
        [Description(",")] Read,


    }

    public class EnumExtender{
        public EnumExtender() {
        
        }

        //source https://www.codingame.com/playgrounds/2487/c---how-to-display-friendly-names-for-enumerations
        public static string getDescriptionOf(Enum e) {
            MemberInfo[] memberInfo = e.GetType().GetMember(e.ToString());
            if ((memberInfo != null && memberInfo.Length > 0))
            {
                var _Attribs = memberInfo[0].GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
                if ((_Attribs != null && _Attribs.Count() > 0))
                {
                    return ((System.ComponentModel.DescriptionAttribute)_Attribs.ElementAt(0)).Description;
                }
            }
            return "";
        } 
    }

    public class BfInterpreter {
        FileLoader fileLoader;
        short[] memory { get; }
        //stack source https://www.tutorialsteacher.com/csharp/csharp-stack
        private Stack<int> loopLointer;
        private string program;

        public BfInterpreter() {
            this.fileLoader = new FileLoader();
            this.memory = new short[30000];
            this.loopLointer = new Stack<int>();
        }

        public void loadProgram(string filename) {
            this.program = this.fileLoader.Load(filename);
        }

        public void interpret() {
            for (int i = 0; i < this.program.Length; i++) { 
                /// if program[i] in enum operate
            }
        }
    }
}