using Datalaag;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace Logica {
    /// <summary>
    /// enumeraties voor valid brainfuck commandos
    /// </summary>
    /// <see cref="https://stackoverflow.com/questions/2373986/how-can-i-use-a-special-char-in-a-c-sharp-enum"/>
    /// <see cref="https://stackoverflow.com/questions/13099834/how-to-get-the-display-name-attribute-of-an-enum-member-via-mvc-razor-code"/>
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
        public EnumExtender() {}

        /// <summary>
        /// return de beschrijving die bij een enum hoort
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        /// <see cref="https://www.codingame.com/playgrounds/2487/c---how-to-display-friendly-names-for-enumerations"></see>
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

        /// <summary>
        /// match een enum met een meegegeven description, return deze
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="description"></param>
        /// <returns>Enum</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <see cref="https://stackoverflow.com/questions/4367723/get-enum-from-description-attribute"></see>
        public static T descriptionToEnum<T>(string description) where T : Enum {
            foreach (var field in typeof(T).GetFields()) {
                if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute) {
                    if (attribute.Description == description)
                        return (T)field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return (T)field.GetValue(null);
                }
            }
            throw new ArgumentException("Not found.", nameof(description));
        }
    }

    public class BfInterpreter {
        FileLoader fileLoader;
        short[] memory { get; }
        /// <see cref="https://www.tutorialsteacher.com/csharp/csharp-stack"></see>
        private Stack<int> loopLointer;
        private string program;
        private Action<string> inputFunction, outputFunction;


        public BfInterpreter(Action<string> inputFunction, Action<string> outputFunction) {
            this.fileLoader = new FileLoader();
            this.memory = new short[30000];
            this.loopLointer = new Stack<int>();
            this.inputFunction = inputFunction;
            this.outputFunction = outputFunction;
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