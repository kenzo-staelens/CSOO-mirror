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
    public enum Commands {
        [Description("+")] Inc,
        [Description("-")] Dec,
        [Description("<")] Left,
        [Description(">")] Right,
        [Description("[")] Loop,
        [Description("]")] Jmp,
        [Description(".")] Write,
        [Description(",")] Read,
    }

    public class EnumExtender {
        public EnumExtender() { }

        /// <summary>
        /// return de beschrijving die bij een enum hoort
        /// </summary>
        /// <param name="e">meegegeven enum waarde</param>
        /// <returns>beschrijving die bij de enum hoort</returns>
        /// <see cref="https://www.codingame.com/playgrounds/2487/c---how-to-display-friendly-names-for-enumerations"></see>
        public static string GetDescriptionOf(Enum e) {
            MemberInfo[] memberInfo = e.GetType().GetMember(e.ToString());
            if ((memberInfo != null && memberInfo.Length > 0)) {
                var _Attribs = memberInfo[0].GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
                if ((_Attribs != null && _Attribs.Count() > 0)) {
                    return ((System.ComponentModel.DescriptionAttribute)_Attribs.ElementAt(0)).Description;
                }
            }
            return "";
        }

        /// <summary>
        /// match een enum met een meegegeven description, return deze
        /// </summary>
        /// <typeparam name="T">enum klasse</typeparam>
        /// <param name="description">meegegeven beschrijving</param>
        /// <returns>Enum</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <see cref="https://stackoverflow.com/questions/4367723/get-enum-from-description-attribute"></see>
        public static T DescriptionToEnum<T>(string description) where T : Enum {
            foreach (var field in typeof(T).GetFields()) {
                if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute) {
                    if (attribute.Description == description)
                        return (T)field.GetValue(null);
                }
                else {
                    if (field.Name == description)
                        return (T)field.GetValue(null);
                }
            }
            throw new ArgumentException("Not found.", nameof(description));
        }
    }

    public class BfInterpreter {
        private FileLoader fileLoader;
        private byte[] memory { get; }
        private Int16 memoryPointer;
        /// <see cref="https://www.tutorialsteacher.com/csharp/csharp-stack"></see>
        private Stack<int> loopPointer;
        int inputPointer;
        private string program;
        private char[] preparedInput;
        private Func<char> inputFunction;
        private Action<string> outputFunction;

        /// <summary>
        /// setup interne parameters voor programma
        /// </summary>
        /// <param name="inputFunction">funcie die wordt uitgeroepen bij "," commando</param>
        /// <param name="outputFunction">funcie die wordt uitgeroepen bij "." commando</param>
        public BfInterpreter(Func<char> inputFunction, Action<string> outputFunction) {
            this.fileLoader = new FileLoader();
            this.memory = new byte[Int16.MaxValue];
            this.memoryPointer = 0;
            this.loopPointer = new Stack<int>();
            this.inputPointer = 0;

            this.inputFunction = inputFunction;
            this.outputFunction = outputFunction;
            this.program = "";
            this.preparedInput = new char[0];
        }

        /// <summary>
        /// laad een brainfuck programma van string of file
        /// </summary>
        /// <param name="programinput"></param>
        /// <see cref="https://stackoverflow.com/questions/3137097/check-if-a-string-is-a-valid-windows-directory-folder-path"></see>
        public void LoadProgram(string programinput) {
            try {
                Path.GetFullPath(programinput);
                this.program = this.fileLoader.Load(programinput);
            }
            catch (FileNotFoundException e){
                this.outputFunction("file " + programinput + " could not be found");
            }
            catch (Exception e) {
                this.program = programinput;
            }
        }

        /// <summary>
        /// zet een predefined input voor het uit te voeren programma, als te weinig input is gegeven wordt deze later gevraagd
        /// </summary>
        /// <param name="input">meegegeven programma in stringvorm of file path</param>
        public void PrepareInput(string input) {
            this.preparedInput = new char[input.Length];
            foreach (char c in input) {
                this.preparedInput[inputPointer] = c;
                inputPointer++;
            }
            this.inputPointer = 0;
        }

        /// <summary>
        /// het uitvoeren van het brainfuck programma gebeurt hier
        /// </summary>
        /// <see cref="https://www.w3schools.com/cs/cs_switch.php"/>
        /// <see cref="https://www.c-sharpcorner.com/UploadFile/mahesh/convert-char-to-byte-in-C-Sharp/"></see>
        public void Interpret() {
            for (int i = 0; i < this.program.Length; i++) {
                Commands cmd = EnumExtender.DescriptionToEnum<Commands>(this.program[i].ToString());
                switch (cmd) {
                    case Commands.Inc:
                        this.memory[memoryPointer]++;
                        break;
                    case Commands.Dec:
                        this.memory[memoryPointer]--;
                        break;
                    case Commands.Right:
                        this.memoryPointer++;
                        break;
                    case Commands.Left:
                        this.memoryPointer--;
                        break;
                    case Commands.Loop:
                        int bracketCount = 0;
                        if (this.memory[memoryPointer] != 0) {
                            this.loopPointer.Push(i);
                        }
                        else {
                            //find index of matching ], dit codeblok hoort namelijk niet te worden uitgevoerd
                            for (int j = i; j < this.program.Length; j++) {
                                if (this.program[i].ToString() == EnumExtender.GetDescriptionOf(Commands.Loop)) bracketCount++;
                                if (this.program[i].ToString() == EnumExtender.GetDescriptionOf(Commands.Jmp)) bracketCount--;

                            }
                        }
                        break;
                    case Commands.Jmp:
                        if (memory[memoryPointer] != 0) i = this.loopPointer.Pop() - 1;
                        break;
                    case Commands.Read:
                        if (inputPointer < this.preparedInput.Length) {
                            this.memory[memoryPointer] = Convert.ToByte(this.preparedInput[inputPointer]);
                        }
                        else {
                            this.memory[memoryPointer] = Convert.ToByte(inputFunction());
                        }
                        break;
                    case Commands.Write:
                        string outp = ((char)this.memory[memoryPointer]).ToString();
                        outputFunction(outp);
                        break;
                }
            }
        }
    }
}