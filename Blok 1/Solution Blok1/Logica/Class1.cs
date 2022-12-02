using Datalaag;
using Globals;
using System.Security.Cryptography;

namespace Logica {
    public class BfInterpreter {
        private FileLoader fileLoader;
        /// <see cref="https://www.tutorialsteacher.com/csharp/csharp-stack"></see>
        private Stack<int> loopPointer;
        private int inputPointer;
        private Commands[] program;
        
        public byte[] Memory { get; private set; }
        public Int16 MemoryPointer { get; private set; }
        public Commands[] Program { get; private set; }
        public char[] PreparedInput { get; set; }
        public Func<char> InputFunction { private get;  set; }
        public Action<string> OutputFunction { private get;  set; }
        public Func<string> SerializedTick { private get; set; }


        /// <summary>
        /// setup interne parameters voor programma
        /// </summary>
        /// <param name="inputFunction">funcie die wordt uitgeroepen bij "," commando</param>
        /// <param name="outputFunction">funcie die wordt uitgeroepen bij "." commando</param>
        public BfInterpreter(Func<char> inputFunction, Action<string> outputFunction) {
            this.fileLoader = new FileLoader();
            this.Memory = new byte[Int16.MaxValue];
            this.MemoryPointer = 0;
            this.loopPointer = new Stack<int>();
            this.inputPointer = 0;

            this.InputFunction = inputFunction;
            this.OutputFunction = outputFunction;
            this.Program = new Commands[0];
            this.PreparedInput = new char[0];
        }

        /// <summary>
        /// laad een brainfuck programma van string of file
        /// </summary>
        /// <param name="programinput"></param>
        /// <see cref="https://stackoverflow.com/questions/3137097/check-if-a-string-is-a-valid-windows-directory-folder-path"></see>
        public void LoadProgram(string programinput) {
            string loadedProgram="";
            try {
                Path.GetFullPath(programinput);
                loadedProgram = this.fileLoader.Load(programinput);
            }
            catch (FileNotFoundException e){
                this.OutputFunction("file " + programinput + " could not be found");
            }
            catch (Exception e) {
                loadedProgram = programinput;
            }
            this.Program = BrainfuckPrecompiler.SimpleEncoding(loadedProgram);
        }

        /// <summary>
        /// zet een predefined input voor het uit te voeren programma, als te weinig input is gegeven wordt deze later gevraagd
        /// </summary>
        /// <param name="input">meegegeven programma in stringvorm of file path</param>
        public void PrepareInput(string input) {
            this.PreparedInput = new char[input.Length];
            foreach (char c in input) {
                this.PreparedInput[inputPointer] = c;
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
            for (int i = 0; i < this.Program.Length; i++) {
                Commands cmd = Program[i];
                if(MemoryPointer<0 || MemoryPointer>this.Memory.Length) throw new IndexOutOfRangeException("tried to access index " + MemoryPointer.ToString() + " of memory with size " + Memory.Length.ToString());
                switch (cmd) {
                    case Commands.Inc:
                        this.Memory[MemoryPointer]++;
                        break;
                    case Commands.Dec:
                        this.Memory[MemoryPointer]--;
                        break;
                    case Commands.Right:
                        this.MemoryPointer++;
                        break;
                    case Commands.Left:
                        this.MemoryPointer--;
                        break;
                    case Commands.Loop:
                        int bracketCount = 0;
                        if (this.Memory[MemoryPointer] != 0) {
                            this.loopPointer.Push(i);
                        }
                        else {
                            //find index of matching ], dit codeblok hoort namelijk niet te worden uitgevoerd
                            for (int j = i; j < this.Program.Length; j++) {
                                if (this.Program[i] == Commands.Loop) bracketCount++;
                                if (this.Program[i]== Commands.Jmp) bracketCount--;
                            }
                        }
                        break;
                    case Commands.Jmp:
                        if (Memory[MemoryPointer] != 0) i = this.loopPointer.Pop() - 1;
                        break;
                    case Commands.Read:
                        if (inputPointer < this.PreparedInput.Length) {
                            this.Memory[MemoryPointer] = Convert.ToByte(this.PreparedInput[inputPointer]);
                        }
                        else {
                            this.Memory[MemoryPointer] = Convert.ToByte(InputFunction());
                        }
                        break;
                    case Commands.Write:
                        string outp = ((char)this.Memory[MemoryPointer]).ToString();
                        OutputFunction(outp);
                        break;
                }
            }
        }
    }
}
